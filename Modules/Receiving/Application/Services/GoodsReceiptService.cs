using Microsoft.EntityFrameworkCore;
using verii_metivon_api.Core.Auth;
using verii_metivon_api.Core.Paging;
using verii_metivon_api.Core.UnitOfWork;
using verii_metivon_api.Modules.Inventory.Application.Services;
using verii_metivon_api.Modules.Inventory.Domain.Entities;
using verii_metivon_api.Modules.Inventory.Domain.Enums;
using verii_metivon_api.Modules.Procurement.Domain.Entities;
using verii_metivon_api.Modules.Procurement.Domain.Enums;
using verii_metivon_api.Modules.Products.Domain.Enums;
using verii_metivon_api.Modules.Receiving.Application.Parameters;
using verii_metivon_api.Modules.Receiving.Domain.Entities;
using verii_metivon_api.Modules.Receiving.Domain.Enums;
using verii_metivon_api.Modules.Traceability.Application.Parameters;
using verii_metivon_api.Modules.NumberSeries.Application;
using verii_metivon_api.Modules.TradeOperations.Domain.Entities;

namespace verii_metivon_api.Modules.Receiving.Application.Services;

public sealed class GoodsReceiptService(IUnitOfWork u,IInventoryService inventory,IReceivingParameterService parameters,IInventoryTraceabilityParameterService traceability,INumberSeriesService numberSeries):IGoodsReceiptService
{
    public async Task<ApiResponse<PagedResult<GoodsReceiptRow>>>GetPagedAsync(GoodsReceiptQuery q,CancellationToken ct)
    {
        var x=u.Repository<GoodsReceipt>().Query().Include(v=>v.Warehouse).Include(v=>v.PurchaseOrder).AsQueryable();
        if(q.PurchaseOrderId.HasValue)x=x.Where(v=>v.PurchaseOrderId==q.PurchaseOrderId||v.PurchaseOrders.Any(link=>link.PurchaseOrderId==q.PurchaseOrderId));if(q.SupplierId.HasValue)x=x.Where(v=>v.SupplierId==q.SupplierId);if(q.WarehouseId.HasValue)x=x.Where(v=>v.WarehouseId==q.WarehouseId);if(q.Status.HasValue)x=x.Where(v=>v.Status==q.Status);
        if(!string.IsNullOrWhiteSpace(q.Search)){var s=q.Search.Trim();x=x.Where(v=>v.ReceiptNumber.Contains(s)||(v.SupplierDeliveryNoteNumber!=null&&v.SupplierDeliveryNoteNumber.Contains(s)));}
        x=x.ApplyPagedFilters(q);
        var total=await x.CountAsync(ct);var suppliers=u.Repository<verii_metivon_api.Core.Domain.BusinessPartner>().Query();
        var rows=await x.GroupJoin(suppliers,a=>a.SupplierId,b=>(long?)b.Id,(a,b)=>new{a,b}).SelectMany(v=>v.b.DefaultIfEmpty(),(v,s)=>new{v.a,s}).OrderByDescending(v=>v.a.ReceiptDate).Skip((q.NormalizedPageNumber-1)*q.NormalizedPageSize).Take(q.NormalizedPageSize).Select(v=>new GoodsReceiptRow(v.a.Id,v.a.ReceiptNumber,v.a.ReceiptType.ToString(),v.s!=null?v.s.Name:null,v.a.Warehouse.Code,v.a.ReceiptDate,v.a.PurchaseOrder!=null?v.a.PurchaseOrder.OrderNumber:null,v.a.Status.ToString())).ToListAsync(ct);
        return ApiResponse<PagedResult<GoodsReceiptRow>>.Ok(new(rows,q.NormalizedPageNumber,q.NormalizedPageSize,total));
    }

    public async Task<ApiResponse<object>>CreateAsync(CreateGoodsReceiptRequest r,CancellationToken ct)
    {
        if(r.Lines.Count==0)return ApiResponse<object>.Error("At least one receipt line is required.",400);
        var settings=await parameters.ResolveSettingsAsync(r.BranchId,r.WarehouseId,ct);
        var purchaseOrderIds=r.PurchaseOrderIds.Where(x=>x>0).Distinct().ToArray();
        var receiptType=r.ReceiptType==GoodsReceiptType.PurchaseOrder&&purchaseOrderIds.Length==0
            ? GoodsReceiptType.FreeReceipt
            : r.ReceiptType;
        var purchaseOrders=purchaseOrderIds.Length==0
            ? []
            : await u.Repository<PurchaseOrder>().Query().Include(x=>x.Lines).Where(x=>purchaseOrderIds.Contains(x.Id)).ToListAsync(ct);
        if(purchaseOrders.Count!=purchaseOrderIds.Length)return ApiResponse<object>.Error("One or more selected purchase orders could not be found.",400);
        if(purchaseOrders.Any(x=>x.BranchId!=r.BranchId||x.SupplierId!=r.SupplierId||x.WarehouseId!=r.WarehouseId))return ApiResponse<object>.Error("All selected purchase orders must belong to the selected branch, supplier and warehouse.",409);
        if(purchaseOrders.Any(x=>x.Status is PurchaseOrderStatus.Cancelled or PurchaseOrderStatus.Received or PurchaseOrderStatus.Invoiced))return ApiResponse<object>.Error("Cancelled, fully received or invoiced purchase orders cannot be received.",409);
        var linkedTradeDossierIds=purchaseOrders.Where(x=>x.TradeDossierId.HasValue).Select(x=>x.TradeDossierId!.Value).Distinct().ToArray();
        if(linkedTradeDossierIds.Length>1)return ApiResponse<object>.Error("Purchase orders linked to different trade dossiers cannot be combined in one goods receipt.",409);
        long? effectiveTradeDossierId=r.TradeDossierId??(linkedTradeDossierIds.Length==1?linkedTradeDossierIds[0]:null);
        if(r.TradeDossierId.HasValue&&linkedTradeDossierIds.Any(x=>x!=r.TradeDossierId.Value))return ApiResponse<object>.Error("The goods receipt trade dossier must match every selected purchase order.",409);
        var selectedOrderLineIds=purchaseOrders.SelectMany(x=>x.Lines).Select(x=>x.Id).ToHashSet();
        if(r.Lines.Any(x=>x.PurchaseOrderLineId.HasValue&&!selectedOrderLineIds.Contains(x.PurchaseOrderLineId.Value)))return ApiResponse<object>.Error("Every purchase order line must belong to one of the selected purchase orders.",409);
        if(purchaseOrderIds.Length>0&&r.Lines.Any(x=>!x.PurchaseOrderLineId.HasValue))return ApiResponse<object>.Error("Every line of an order-based goods receipt must reference a selected purchase order line.",400);
        if(purchaseOrderIds.Length==0&&r.Lines.Any(x=>x.PurchaseOrderLineId.HasValue))return ApiResponse<object>.Error("A free goods receipt cannot reference a purchase order line.",400);
        var purchaseOrderLines=purchaseOrders.SelectMany(x=>x.Lines).ToDictionary(x=>x.Id);
        foreach(var requestedLineGroup in r.Lines.Where(x=>x.PurchaseOrderLineId.HasValue).GroupBy(x=>x.PurchaseOrderLineId!.Value))
        {
            var orderLine=purchaseOrderLines[requestedLineGroup.Key];
            if(requestedLineGroup.Any(x=>x.ProductId!=orderLine.ProductId||x.UnitId!=orderLine.UnitId))return ApiResponse<object>.Error("The product and unit must match the selected purchase order line.",409);
            var requestedQuantity=requestedLineGroup.Sum(x=>x.ReceivedQuantity);
            var overTolerance=orderLine.OverDeliveryTolerance>0?orderLine.OverDeliveryTolerance:settings.OverDeliveryTolerancePercent;
            var maximumQuantity=orderLine.OrderedQuantity*(1+overTolerance/100m);
            if(orderLine.ReceivedQuantity+requestedQuantity>maximumQuantity)return ApiResponse<object>.Error("The receipt quantity exceeds the remaining purchase order quantity and its over-delivery tolerance.",409);
        }
        TradeDossier? trade=null;long? forcedTradeStatusId=null;
        if(effectiveTradeDossierId.HasValue){trade=await u.Repository<TradeDossier>().GetByIdAsync(effectiveTradeDossierId.Value,ct);if(trade is null||trade.Direction!=TradeDirection.Import)return ApiResponse<object>.Error("A valid import dossier is required.",400);if(trade.Status is TradeDossierStatus.Closed or TradeDossierStatus.Cancelled)return ApiResponse<object>.Error("Closed or cancelled trade dossier cannot receive goods.",409);if(trade.BranchId!=r.BranchId||r.SupplierId.HasValue&&trade.BusinessPartnerId!=r.SupplierId.Value)return ApiResponse<object>.Error("Trade dossier branch and supplier must match the goods receipt.",409);var statusCode=trade.BondedWarehouseId.HasValue?"BONDED":"CUSTOMS_HOLD";forcedTradeStatusId=await u.Repository<InventoryStatus>().Query().Where(x=>x.Code==statusCode&&x.IsActive).Select(x=>(long?)x.Id).FirstOrDefaultAsync(ct);if(!forcedTradeStatusId.HasValue)return ApiResponse<object>.Error($"Required inventory status {statusCode} is missing.",409);}
        if(settings.RequireSupplierDeliveryNoteNumber&&string.IsNullOrWhiteSpace(r.SupplierDeliveryNoteNumber))return ApiResponse<object>.Error("Supplier delivery note number is required by receiving parameters.",400);
        var requestSerials=new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach(var line in r.Lines)
        {
            if(line.ReceivedQuantity<=0||line.AcceptedQuantity<0||line.RejectedQuantity<0||line.AcceptedQuantity+line.RejectedQuantity!=line.ReceivedQuantity)return ApiResponse<object>.Error("Receipt quantities are inconsistent.",400);
            var tracking=await u.Repository<verii_metivon_api.Modules.Products.Domain.Entities.Product>().Query().Where(x=>x.Id==line.ProductId&&x.IsActive).Select(x=>x.TrackingType).FirstOrDefaultAsync(ct);
            var serials=(line.SerialNumbers??Array.Empty<string>()).Where(s=>!string.IsNullOrWhiteSpace(s)).Select(s=>s.Trim()).ToArray();
            if(tracking==InventoryTrackingType.Serial&&(line.AcceptedQuantity!=decimal.Truncate(line.AcceptedQuantity)||serials.Distinct(StringComparer.OrdinalIgnoreCase).Count()!=(int)line.AcceptedQuantity))return ApiResponse<object>.Error("For serial-tracked products, accepted quantity must be an integer and unique serial count must match accepted quantity.",400);
            if(tracking!=InventoryTrackingType.Serial&&serials.Length>0)return ApiResponse<object>.Error("Serial numbers can only be entered for serial-tracked products.",400);
            foreach(var serial in serials){var key=$"{line.ProductId}:{serial}";if(!requestSerials.Add(key))return ApiResponse<object>.Error($"Duplicate serial number in receipt: {serial}",400);}
        }
        ReservedNumber? reserved=null;string number=string.Empty;
        try
        {
            var requestedNumber=r.ReceiptNumber?.Trim();
            if(string.IsNullOrWhiteSpace(requestedNumber))
            {
                const int maximumAutomaticNumberAttempts=100;
                for(var attempt=0;attempt<maximumAutomaticNumberAttempts;attempt++)
                {
                    var candidate=await numberSeries.ReserveAsync(new("Receiving","GoodsReceiptNumber",null,r.BranchId,r.WarehouseId,null,r.SupplierId,null,null,r.ReceiptDate,"GoodsReceipt"),ct);
                    if(!await u.Repository<GoodsReceipt>().ExistsAsync(x=>x.ReceiptNumber==candidate.DocumentNumber,ct)){reserved=candidate;number=candidate.DocumentNumber;break;}
                    await numberSeries.CancelAsync(candidate.UsageId,"AUTO_SKIPPED_EXISTING_GOODS_RECEIPT_NUMBER",ct);
                }
                if(reserved is null)return ApiResponse<object>.Error("A unique goods receipt number could not be generated. Review the assigned number series.",409);
            }
            else
            {
                number=await parameters.ResolveNumberAsync(requestedNumber,r.BranchId,ct);
                if(await u.Repository<GoodsReceipt>().ExistsAsync(x=>x.ReceiptNumber==number,ct))return ApiResponse<object>.Error("Goods receipt number already exists.",409);
            }
        }
        catch(InvalidOperationException exception){return ApiResponse<object>.Error(exception.Message,400);}
        var e=new GoodsReceipt{ReceiptNumber=number,ReceiptType=receiptType,Status=GoodsReceiptStatus.Draft,BranchId=r.BranchId,SupplierId=r.SupplierId,PurchaseOrderId=purchaseOrderIds.Length>0?purchaseOrderIds[0]:null,TradeDossierId=effectiveTradeDossierId,WarehouseId=r.WarehouseId,SupplierDeliveryNoteNumber=r.SupplierDeliveryNoteNumber?.Trim(),ReceiptDate=r.ReceiptDate,Notes=r.Notes};
        var n=0;foreach(var l in r.Lines)
        {
            if(l.ReceivedQuantity<=0||l.AcceptedQuantity<0||l.RejectedQuantity<0||l.AcceptedQuantity+l.RejectedQuantity!=l.ReceivedQuantity)return ApiResponse<object>.Error("Receipt quantities are inconsistent.",400);
            var tracking=await u.Repository<verii_metivon_api.Modules.Products.Domain.Entities.Product>().Query().Where(x=>x.Id==l.ProductId&&x.IsActive).Select(x=>x.TrackingType).FirstOrDefaultAsync(ct);
            var serials=(l.SerialNumbers??Array.Empty<string>()).Where(s=>!string.IsNullOrWhiteSpace(s)).Select(s=>s.Trim()).Distinct(StringComparer.OrdinalIgnoreCase).ToArray();
            if(tracking==InventoryTrackingType.Serial&&(l.AcceptedQuantity!=decimal.Truncate(l.AcceptedQuantity)||serials.Length!=(int)l.AcceptedQuantity))return ApiResponse<object>.Error("For serial-tracked products, accepted quantity must be an integer and serial count must match accepted quantity.",400);
            if(tracking!=InventoryTrackingType.Serial&&serials.Length>0)return ApiResponse<object>.Error("Serial numbers can only be entered for serial-tracked products.",400);
            e.Lines.Add(new GoodsReceiptLine{LineNumber=++n,PurchaseOrderLineId=l.PurchaseOrderLineId,ProductId=l.ProductId,UnitId=l.UnitId,StorageLocationId=l.StorageLocationId,InventoryStatusId=forcedTradeStatusId??l.InventoryStatusId,ExpectedQuantity=l.ExpectedQuantity,ReceivedQuantity=l.ReceivedQuantity,AcceptedQuantity=l.AcceptedQuantity,RejectedQuantity=l.RejectedQuantity,UnitCost=l.UnitCost,LotNumber=l.LotNumber?.Trim(),ManufactureDate=l.ManufactureDate,ExpiryDate=l.ExpiryDate,Notes=l.Notes,Serials=serials.Select(s=>new GoodsReceiptSerial{SerialNumber=s}).ToList()});
        }
        try
        {
            await u.ExecuteInTransactionAsync(async t=>
            {
                await u.Repository<GoodsReceipt>().AddAsync(e,t);await u.SaveChangesAsync(t);
                foreach(var orderId in purchaseOrderIds)await u.Repository<GoodsReceiptPurchaseOrder>().AddAsync(new(){GoodsReceiptId=e.Id,PurchaseOrderId=orderId},t);
                if(trade is not null){await u.Repository<TradeDocumentLink>().AddAsync(new(){TradeDossierId=trade.Id,LinkType=TradeLinkType.GoodsReceipt,SourceId=e.Id,ReferenceNumber=e.ReceiptNumber},t);foreach(var line in e.Lines)await u.Repository<TradeDocumentLink>().AddAsync(new(){TradeDossierId=trade.Id,LinkType=TradeLinkType.GoodsReceipt,SourceId=e.Id,SourceLineId=line.Id,LinkedQuantity=line.AcceptedQuantity,ReferenceNumber=e.ReceiptNumber},t);}
                await u.SaveChangesAsync(t);
                if(reserved is not null)await numberSeries.MarkUsedAsync(reserved.UsageId,e.Id,t);
            },ct);
        }
        catch(DbUpdateException){return ApiResponse<object>.Error("The goods receipt could not be saved because another transaction used the same number or relation. Refresh and try again.",409);}
        return ApiResponse<object>.Ok(new{e.Id,e.ReceiptNumber,e.TradeDossierId,PurchaseOrderIds=purchaseOrderIds,NumberSeriesUsageId=reserved?.UsageId});
    }

    public async Task<ApiResponse<object>>PostAsync(long id,CancellationToken ct)
    {
        var receipt=await u.Repository<GoodsReceipt>().Query(true).Include(x=>x.Lines).ThenInclude(x=>x.Serials).FirstOrDefaultAsync(x=>x.Id==id,ct);
        if(receipt is null)return ApiResponse<object>.Error("Goods receipt not found.",404);if(receipt.Status!=GoodsReceiptStatus.Draft&&receipt.Status!=GoodsReceiptStatus.Registered)return ApiResponse<object>.Error("Receipt is not postable.",409);
        var settings=await parameters.ResolveSettingsAsync(receipt.BranchId,receipt.WarehouseId,ct);var trace=await traceability.ResolveSettingsAsync(receipt.BranchId,receipt.WarehouseId,ct);ApiResponse<PostInventoryResult>? posting=null;
        await u.ExecuteInTransactionAsync(async t=>
        {
            var postLines=new List<PostInventoryLine>();
            var affectedOrderIds=new HashSet<long>();
            foreach(var line in receipt.Lines)
            {
                var product=await u.Repository<verii_metivon_api.Modules.Products.Domain.Entities.Product>().GetByIdAsync(line.ProductId,t)??throw new InvalidOperationException("Product not found.");long? lotId=null;
                if(product.TrackingType==InventoryTrackingType.Lot)
                {
                    if((settings.RequireLotNumberForLotTracked||trace.RequireLotForLotTrackedProducts)&&string.IsNullOrWhiteSpace(line.LotNumber))throw new InvalidOperationException("Lot number is required by inventory traceability parameters.");
                    if((settings.RequireExpiryDateForTrackedItems||(trace.RequireExpiryDateForShelfLifeProducts&&product.ShelfLifeDays.HasValue))&&!line.ExpiryDate.HasValue)throw new InvalidOperationException("Expiry date is required by inventory traceability parameters.");
                    if(trace.RequireManufactureDateWhenExpiryExists&&line.ExpiryDate.HasValue&&!line.ManufactureDate.HasValue)throw new InvalidOperationException("Manufacture date is required when an expiry date is entered.");
                    if(line.ManufactureDate.HasValue&&line.ExpiryDate.HasValue&&line.ManufactureDate.Value>line.ExpiryDate.Value)throw new InvalidOperationException("Manufacture date cannot be after the expiry date.");
                    if(trace.RejectExpiredReceipts&&line.ExpiryDate.HasValue&&line.ExpiryDate.Value<receipt.ReceiptDate)throw new InvalidOperationException("Expired products cannot be received.");
                    var minimumDays=Math.Max(settings.MinimumRemainingShelfLifeDays,trace.MinimumRemainingShelfLifeDays);if(line.ExpiryDate.HasValue&&line.ExpiryDate.Value<receipt.ReceiptDate.AddDays(minimumDays))throw new InvalidOperationException($"Expiry date must provide at least {minimumDays} days of remaining shelf life.");
                    if(trace.MinimumRemainingShelfLifePercent>0&&product.ShelfLifeDays is >0&&line.ExpiryDate.HasValue){var remaining=line.ExpiryDate.Value.DayNumber-receipt.ReceiptDate.DayNumber;var percent=remaining*100m/product.ShelfLifeDays.Value;if(percent<trace.MinimumRemainingShelfLifePercent)throw new InvalidOperationException($"Remaining shelf life must be at least {trace.MinimumRemainingShelfLifePercent:0.##}%.");}
                    if(string.IsNullOrWhiteSpace(line.LotNumber))throw new InvalidOperationException("A lot number is required to create the inventory lot.");
                    var lot=await u.Repository<InventoryLot>().FirstOrDefaultAsync(x=>x.ProductId==line.ProductId&&x.LotNumber==line.LotNumber,true,t);
                    if(lot is null){lot=new InventoryLot{ProductId=line.ProductId,LotNumber=line.LotNumber,ManufactureDate=line.ManufactureDate,ExpiryDate=line.ExpiryDate};await u.Repository<InventoryLot>().AddAsync(lot,t);await u.SaveChangesAsync(t);}lotId=lot.Id;
                    postLines.Add(new(line.ProductId,line.UnitId,receipt.WarehouseId,line.StorageLocationId,line.InventoryStatusId,lotId,null,line.AcceptedQuantity,line.UnitCost,line.Id,line.Notes));
                }
                else if(product.TrackingType==InventoryTrackingType.Serial)
                {
                    if((settings.RequireSerialsForSerialTracked||trace.RequireSerialForSerialTrackedProducts)&&line.Serials.Count!=(int)line.AcceptedQuantity)throw new InvalidOperationException("Serial count must match accepted quantity.");
                    foreach(var source in line.Serials){if(trace.PreventDuplicateSerialNumbers&&await u.Repository<InventorySerial>().ExistsAsync(x=>x.ProductId==line.ProductId&&x.SerialNumber==source.SerialNumber,t))throw new InvalidOperationException($"Serial number already exists: {source.SerialNumber}");var serial=new InventorySerial{ProductId=line.ProductId,SerialNumber=source.SerialNumber};await u.Repository<InventorySerial>().AddAsync(serial,t);await u.SaveChangesAsync(t);postLines.Add(new(line.ProductId,line.UnitId,receipt.WarehouseId,line.StorageLocationId,line.InventoryStatusId,null,serial.Id,1,line.UnitCost,line.Id,line.Notes));}
                }
                else postLines.Add(new(line.ProductId,line.UnitId,receipt.WarehouseId,line.StorageLocationId,line.InventoryStatusId,null,null,line.AcceptedQuantity,line.UnitCost,line.Id,line.Notes));

                if(line.PurchaseOrderLineId.HasValue)
                {
                    var poLine=await u.Repository<PurchaseOrderLine>().GetByIdForUpdateAsync(line.PurchaseOrderLineId.Value,t)??throw new InvalidOperationException("Purchase order line not found.");
                    affectedOrderIds.Add(poLine.PurchaseOrderId);
                    var overTolerance=poLine.OverDeliveryTolerance>0?poLine.OverDeliveryTolerance:settings.OverDeliveryTolerancePercent;var newTotal=poLine.ReceivedQuantity+line.ReceivedQuantity;var max=poLine.OrderedQuantity*(1+overTolerance/100m);
                    if(newTotal>max)throw new InvalidOperationException("Over-delivery tolerance exceeded.");if(!settings.AllowPartialReceipt&&newTotal<poLine.OrderedQuantity)throw new InvalidOperationException("Partial receipt is disabled by receiving parameters.");
                    poLine.ReceivedQuantity=newTotal;var minimumComplete=poLine.OrderedQuantity*(1-settings.UnderDeliveryTolerancePercent/100m);poLine.Status=newTotal>=minimumComplete?PurchaseLineStatus.Received:PurchaseLineStatus.PartiallyReceived;poLine.IsClosed=poLine.Status==PurchaseLineStatus.Received;
                }
            }
            posting=await inventory.PostAsync(new($"GOODS-RECEIPT:{receipt.Id}","GoodsReceipt",receipt.Id,receipt.ReceiptNumber,receipt.ReceiptDate.ToDateTime(TimeOnly.MinValue),InventoryMovementType.PurchaseReceipt,InventoryMovementDirection.Receipt,settings.InventoryCurrencyCode,postLines),t);
            if(!posting.Success)throw new InvalidOperationException(posting.Message);receipt.InventoryPostingId=posting.Data!.PostingId;receipt.Status=settings.RequireQualityInspection?GoodsReceiptStatus.QualityInspection:GoodsReceiptStatus.Posted;receipt.PostedAt=DateTime.UtcNow;
            foreach(var orderId in affectedOrderIds){var order=await u.Repository<PurchaseOrder>().Query(true).Include(x=>x.Lines).FirstAsync(x=>x.Id==orderId,t);order.Status=order.Lines.All(x=>x.Status==PurchaseLineStatus.Received)?PurchaseOrderStatus.Received:PurchaseOrderStatus.PartiallyReceived;}
            await u.SaveChangesAsync(t);
        },ct);
        return ApiResponse<object>.Ok(new{receipt.Id,receipt.InventoryPostingId,receipt.Status,LabelsReady=settings.AutoCreateLabels&&trace.AutoCreateLabelsAfterReceipt,LabelCopies=trace.DefaultLabelCopies});
    }
}
