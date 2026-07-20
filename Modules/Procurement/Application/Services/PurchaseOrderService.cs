using Microsoft.EntityFrameworkCore;
using verii_metivon_api.Core.Auth;
using verii_metivon_api.Core.Paging;
using verii_metivon_api.Core.Persistence;
using verii_metivon_api.Core.UnitOfWork;
using verii_metivon_api.Modules.GeneralSettings;
using verii_metivon_api.Modules.Procurement.Application.Parameters;
using verii_metivon_api.Modules.Procurement.Domain.Entities;
using verii_metivon_api.Modules.Procurement.Domain.Enums;
using verii_metivon_api.Modules.NumberSeries.Application;
using verii_metivon_api.Modules.TradeOperations.Domain.Entities;

namespace verii_metivon_api.Modules.Procurement.Application.Services;

public sealed class PurchaseOrderService(IUnitOfWork u,IProcurementParameterService parameters,MetivonDbContext db,INumberSeriesService numberSeries):IPurchaseOrderService
{
 public async Task<ApiResponse<PagedResult<PurchaseOrderRow>>>GetPagedAsync(PurchaseOrderQuery q,CancellationToken ct){var x=u.Repository<PurchaseOrder>().Query().Include(v=>v.Supplier).Include(v=>v.Warehouse).Include(v=>v.Currency).AsQueryable();if(q.SupplierId.HasValue)x=x.Where(v=>v.SupplierId==q.SupplierId);if(q.WarehouseId.HasValue)x=x.Where(v=>v.WarehouseId==q.WarehouseId);if(q.Status.HasValue)x=x.Where(v=>v.Status==q.Status);if(q.FromDate.HasValue)x=x.Where(v=>v.OrderDate>=q.FromDate);if(q.ToDate.HasValue)x=x.Where(v=>v.OrderDate<=q.ToDate);if(!string.IsNullOrWhiteSpace(q.Search)){var s=q.Search.Trim();x=x.Where(v=>v.OrderNumber.Contains(s)||v.Supplier.Code.Contains(s)||v.Supplier.Name.Contains(s));}x=x.ApplyPagedFilters(q);var total=await x.CountAsync(ct);var rows=await x.OrderByDescending(v=>v.OrderDate).ThenByDescending(v=>v.Id).Skip((q.NormalizedPageNumber-1)*q.NormalizedPageSize).Take(q.NormalizedPageSize).Select(v=>new PurchaseOrderRow(v.Id,v.OrderNumber,v.Supplier.Code,v.Supplier.Name,v.Warehouse.Code,v.OrderDate,v.ConfirmedDeliveryDate??v.RequestedDeliveryDate,v.Currency.Code,v.GrandTotal,v.Status.ToString())).ToListAsync(ct);return ApiResponse<PagedResult<PurchaseOrderRow>>.Ok(new(rows,q.NormalizedPageNumber,q.NormalizedPageSize,total));}
 public async Task<ApiResponse<object>>CreateAsync(CreatePurchaseOrderRequest r,CancellationToken ct)
 {
  var p=await parameters.ResolveSettingsAsync(r.BranchId,r.WarehouseId,ct);
  if(r.Lines is null||r.Lines.Count==0)return ApiResponse<object>.Error("At least one line is required.",400);
  if(r.Lines.Count>p.MaximumLinesPerOrder)return ApiResponse<object>.Error($"Purchase order cannot exceed {p.MaximumLinesPerOrder} lines.",400);
  if(!p.AllowPastOrderDate&&r.OrderDate<DateOnly.FromDateTime(DateTime.UtcNow))return ApiResponse<object>.Error("Past purchase order date is disabled by procurement parameters.",400);
  if(p.RequireSupplierReference&&string.IsNullOrWhiteSpace(r.SupplierReference))return ApiResponse<object>.Error("Supplier reference is required.",400);
  if(p.RequireBuyerNote&&string.IsNullOrWhiteSpace(r.BuyerNote))return ApiResponse<object>.Error("Buyer note is required.",400);
  if(!await db.Branches.AsNoTracking().AnyAsync(x=>x.Id==r.BranchId&&x.IsActive,ct))return ApiResponse<object>.Error("The active branch could not be found.",400);
  if(!await db.Warehouses.AsNoTracking().AnyAsync(x=>x.Id==r.WarehouseId&&x.BranchId==r.BranchId&&x.IsActive,ct))return ApiResponse<object>.Error("The delivery warehouse must be active and belong to the selected branch.",400);
  if(!await db.BusinessPartners.AsNoTracking().AnyAsync(x=>x.Id==r.SupplierId&&x.BranchId==r.BranchId&&(!p.RequireActiveSupplier||x.IsActive),ct))return ApiResponse<object>.Error("The supplier must belong to the active branch and satisfy the procurement parameters.",400);
  var currency=await db.Currencies.AsNoTracking().FirstOrDefaultAsync(x=>x.Id==r.CurrencyId&&x.IsActive,ct);
  if(currency is null)return ApiResponse<object>.Error("An active currency is required.",400);
  var baseCurrencyId=await db.Set<GeneralSetting>().AsNoTracking().Where(x=>x.ScopeKey=="GLOBAL").Select(x=>(long?)x.DefaultCurrencyId).FirstOrDefaultAsync(ct)
      ??await db.Currencies.AsNoTracking().Where(x=>x.IsActive&&x.IsDefault).Select(x=>(long?)x.Id).FirstOrDefaultAsync(ct);
  var effectiveExchangeRate=baseCurrencyId==currency.Id?1m:r.ExchangeRate;
  if(effectiveExchangeRate<=0)return ApiResponse<object>.Error("A positive exchange rate is required.",400);
  if(!await db.PaymentTerms.AsNoTracking().AnyAsync(x=>x.Id==r.PaymentTermId&&x.IsActive,ct))return ApiResponse<object>.Error("An active payment term is required.",400);
  var productIds=r.Lines.Select(x=>x.ProductId).Distinct().ToArray();
  if(await db.Products.AsNoTracking().CountAsync(x=>productIds.Contains(x.Id)&&x.IsActive,ct)!=productIds.Length)return ApiResponse<object>.Error("One or more purchase order products are inactive or could not be found.",400);
  var unitIds=r.Lines.Select(x=>x.UnitId).Distinct().ToArray();
  if(await db.Units.AsNoTracking().CountAsync(x=>unitIds.Contains(x.Id)&&x.IsActive,ct)!=unitIds.Length)return ApiResponse<object>.Error("One or more purchase order units are inactive or could not be found.",400);
  var locationIds=r.Lines.Where(x=>x.StorageLocationId.HasValue).Select(x=>x.StorageLocationId!.Value).Distinct().ToArray();
  if(locationIds.Length>0&&await db.StorageLocations.AsNoTracking().CountAsync(x=>locationIds.Contains(x.Id)&&x.WarehouseId==r.WarehouseId&&x.IsActive&&!x.IsBlocked,ct)!=locationIds.Length)return ApiResponse<object>.Error("Optional stock locations must be active, unblocked and belong to the delivery warehouse.",400);
  TradeDossier? trade=null;
  if(r.TradeDossierId.HasValue){trade=await db.TradeDossiers.AsNoTracking().FirstOrDefaultAsync(x=>x.Id==r.TradeDossierId.Value,ct);if(trade is null||trade.Direction!=TradeDirection.Import)return ApiResponse<object>.Error("A valid import trade dossier is required.",400);if(trade.Status is TradeDossierStatus.Closed or TradeDossierStatus.Cancelled)return ApiResponse<object>.Error("Closed or cancelled trade dossier cannot be linked.",409);if(trade.BranchId!=r.BranchId||trade.BusinessPartnerId!=r.SupplierId)return ApiResponse<object>.Error("Trade dossier branch and supplier must match the purchase order.",409);if(trade.CurrencyId!=r.CurrencyId)return ApiResponse<object>.Error("Trade dossier and purchase order currencies must match.",409);}
  var requestedDelivery=r.RequestedDeliveryDate??r.OrderDate.AddDays(p.DefaultLeadTimeDays);
  if(p.RequireRequestedDeliveryDate&&!r.RequestedDeliveryDate.HasValue&&p.DefaultLeadTimeDays==0)return ApiResponse<object>.Error("Requested delivery date is required.",400);
  ReservedNumber? reserved=null;string number=string.Empty;
  try
  {
   if(string.IsNullOrWhiteSpace(r.OrderNumber))
   {
    const int maximumAutomaticNumberAttempts=100;
    for(var attempt=0;attempt<maximumAutomaticNumberAttempts;attempt++)
    {
     var candidate=await numberSeries.ReserveAsync(new("Procurement","PurchaseOrderNumber",null,r.BranchId,r.WarehouseId,null,r.SupplierId,null,null,r.OrderDate,"PurchaseOrder"),ct);
     if(!await u.Repository<PurchaseOrder>().ExistsAsync(x=>x.OrderNumber==candidate.DocumentNumber,ct)){reserved=candidate;number=candidate.DocumentNumber;break;}
     await numberSeries.CancelAsync(candidate.UsageId,"AUTO_SKIPPED_EXISTING_PURCHASE_ORDER_NUMBER",ct);
    }
    if(reserved is null)return ApiResponse<object>.Error("A unique purchase order number could not be generated. Review the assigned number series.",409);
   }
   else
   {
    number=await parameters.ResolveOrderNumberAsync(r.OrderNumber,r.BranchId,ct);
    if(await u.Repository<PurchaseOrder>().ExistsAsync(x=>x.OrderNumber==number,ct))return ApiResponse<object>.Error("Order number already exists.",409);
   }
  }
  catch(InvalidOperationException exception){return ApiResponse<object>.Error(exception.Message,400);}
  var e=new PurchaseOrder{OrderNumber=number,OrderType=r.OrderType,Status=p.AutoApproveOnCreate?PurchaseOrderStatus.Approved:PurchaseOrderStatus.Draft,BranchId=r.BranchId,SupplierId=r.SupplierId,TradeDossierId=r.TradeDossierId,CurrencyId=r.CurrencyId,PaymentTermId=r.PaymentTermId,WarehouseId=r.WarehouseId,OrderDate=r.OrderDate,RequestedDeliveryDate=requestedDelivery,SupplierReference=r.SupplierReference?.Trim(),BuyerNote=r.BuyerNote?.Trim(),ExchangeRate=effectiveExchangeRate};
  var n=0;
  foreach(var l in r.Lines)
  {
   if(l.Quantity<=0||l.UnitPrice<0||(!p.AllowZeroPrice&&l.UnitPrice==0))return ApiResponse<object>.Error("Invalid quantity or price.",400);
   if(l.DiscountRate<0||l.DiscountRate>100||l.TaxRate<0||l.TaxRate>100)return ApiResponse<object>.Error("Discount and tax rates must be between 0 and 100.",400);
   if(!p.AllowDiscount&&l.DiscountRate>0)return ApiResponse<object>.Error("Discount is disabled by procurement parameters.",400);
   if(l.DiscountRate>p.MaximumDiscountPercent)return ApiResponse<object>.Error($"Discount cannot exceed {p.MaximumDiscountPercent}%.",400);
   var over=l.OverDeliveryTolerance==0?p.DefaultOverDeliveryTolerancePercent:l.OverDeliveryTolerance;var under=l.UnderDeliveryTolerance==0?p.DefaultUnderDeliveryTolerancePercent:l.UnderDeliveryTolerance;
   if(over is <0 or >100||under is <0 or >100)return ApiResponse<object>.Error("Delivery tolerance must be between 0 and 100.",400);
   var amounts=PurchaseOrderAmountCalculator.Calculate(l.Quantity,l.UnitPrice,l.DiscountRate,l.TaxRate);
   e.Lines.Add(new PurchaseOrderLine{LineNumber=++n,ProductId=l.ProductId,UnitId=l.UnitId,StorageLocationId=l.StorageLocationId,Description=l.Description,OrderedQuantity=l.Quantity,UnitPrice=l.UnitPrice,DiscountRate=l.DiscountRate,DiscountAmount=amounts.DiscountAmount,TaxRate=l.TaxRate,TaxAmount=amounts.TaxAmount,LineTotal=amounts.GrandTotal,RequestedDeliveryDate=l.RequestedDeliveryDate??requestedDelivery,OverDeliveryTolerance=over,UnderDeliveryTolerance=under});e.Subtotal+=amounts.GrossAmount;e.DiscountTotal+=amounts.DiscountAmount;e.TaxTotal+=amounts.TaxAmount;e.GrandTotal+=amounts.GrandTotal;
  }
  if(e.GrandTotal<p.MinimumOrderAmount)return ApiResponse<object>.Error($"Purchase order total must be at least {p.MinimumOrderAmount}.",400);
  if(p.MaximumOrderAmount>0&&e.GrandTotal>p.MaximumOrderAmount)return ApiResponse<object>.Error($"Purchase order total cannot exceed {p.MaximumOrderAmount}.",400);
  await u.ExecuteInTransactionAsync(async t=>{await u.Repository<PurchaseOrder>().AddAsync(e,t);await u.SaveChangesAsync(t);if(trade is not null){await u.Repository<TradeDocumentLink>().AddAsync(new(){TradeDossierId=trade.Id,LinkType=TradeLinkType.PurchaseOrder,SourceId=e.Id,ReferenceNumber=e.OrderNumber},t);await u.SaveChangesAsync(t);}if(reserved is not null)await numberSeries.MarkUsedAsync(reserved.UsageId,e.Id,t);},ct);return ApiResponse<object>.Ok(new{e.Id,e.OrderNumber,e.Status,e.TradeDossierId,NumberSeriesUsageId=reserved?.UsageId});
 }
 public async Task<ApiResponse<object>>ConfirmAsync(long id,CancellationToken ct){var e=await u.Repository<PurchaseOrder>().GetByIdForUpdateAsync(id,ct);if(e is null)return ApiResponse<object>.Error("Purchase order not found.",404);var p=await parameters.ResolveSettingsAsync(e.BranchId,e.WarehouseId,ct);if(p.RequireApprovalBeforeConfirmation&&e.Status!=PurchaseOrderStatus.Approved)return ApiResponse<object>.Error("Purchase order approval is required before confirmation.",409);if(!p.RequireApprovalBeforeConfirmation&&e.Status is not(PurchaseOrderStatus.Draft or PurchaseOrderStatus.Approved))return ApiResponse<object>.Error("Only draft or approved orders can be confirmed.",409);e.Status=PurchaseOrderStatus.Confirmed;e.ConfirmedAt=DateTime.UtcNow;await u.SaveChangesAsync(ct);return ApiResponse<object>.Ok(new{e.Id});}
 public async Task<ApiResponse<object>>CancelAsync(long id,CancellationToken ct){var e=await u.Repository<PurchaseOrder>().GetByIdForUpdateAsync(id,ct);if(e is null)return ApiResponse<object>.Error("Purchase order not found.",404);if(e.Status is PurchaseOrderStatus.PartiallyReceived or PurchaseOrderStatus.Received or PurchaseOrderStatus.Invoiced)return ApiResponse<object>.Error("Received orders cannot be cancelled; use a return document.",409);e.Status=PurchaseOrderStatus.Cancelled;await u.SaveChangesAsync(ct);return ApiResponse<object>.Ok(new{e.Id});}
}


