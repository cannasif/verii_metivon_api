using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using verii_metivon_api.Core.Auth;
using verii_metivon_api.Core.Persistence;
using verii_metivon_api.Modules.Receiving.Domain.Enums;
using verii_metivon_api.Modules.Receiving.Application.Services;
using verii_metivon_api.Modules.Receiving.Domain.Entities;

namespace verii_metivon_api.Modules.Receiving.Api;

[ApiController,Authorize,Route("api/goods-receipts/{id:long}")]
public sealed class GoodsReceiptMaintenanceController(MetivonDbContext db):ControllerBase
{
    [HttpGet]
    public async Task<IActionResult>Get(long id,CancellationToken ct)
    {
        var e=await db.GoodsReceipts.AsNoTracking().Include(x=>x.Lines).ThenInclude(x=>x.Serials).Include(x=>x.PurchaseOrders).FirstOrDefaultAsync(x=>x.Id==id,ct);
        if(e is null)return NotFound(ApiResponse<object>.Error("Goods receipt not found.",404));
        return Ok(ApiResponse<object>.Ok(new{e.ReceiptNumber,e.ReceiptType,e.BranchId,e.SupplierId,PurchaseOrderIds=e.PurchaseOrders.Select(x=>x.PurchaseOrderId),e.TradeDossierId,e.WarehouseId,e.SupplierDeliveryNoteNumber,e.ReceiptDate,e.Notes,Lines=e.Lines.OrderBy(x=>x.LineNumber).Select(x=>new{x.PurchaseOrderLineId,x.ProductId,x.UnitId,x.StorageLocationId,x.InventoryStatusId,x.ExpectedQuantity,x.ReceivedQuantity,x.AcceptedQuantity,x.RejectedQuantity,x.UnitCost,x.LotNumber,x.ManufactureDate,x.ExpiryDate,SerialNumbers=x.Serials.Select(s=>s.SerialNumber),x.Notes})}));
    }
    [HttpPut]
    public async Task<IActionResult>Update(long id,CreateGoodsReceiptRequest r,CancellationToken ct)
    {
        var e=await db.GoodsReceipts.Include(x=>x.Lines).ThenInclude(x=>x.Serials).Include(x=>x.PurchaseOrders).FirstOrDefaultAsync(x=>x.Id==id,ct);
        if(e is null)return NotFound(ApiResponse<object>.Error("Goods receipt not found.",404));
        if(e.Status is not (GoodsReceiptStatus.Draft or GoodsReceiptStatus.Registered))return Conflict(ApiResponse<object>.Error("Only unposted goods receipts can be edited.",409));
        if(r.Lines.Count==0)return BadRequest(ApiResponse<object>.Error("At least one receipt line is required.",400));
        e.ReceiptNumber=string.IsNullOrWhiteSpace(r.ReceiptNumber)?e.ReceiptNumber:r.ReceiptNumber.Trim().ToUpperInvariant();e.ReceiptType=r.ReceiptType;e.BranchId=r.BranchId;e.SupplierId=r.SupplierId;e.PurchaseOrderId=r.PurchaseOrderIds.FirstOrDefault() is var first&&first>0?first:null;e.TradeDossierId=r.TradeDossierId;e.WarehouseId=r.WarehouseId;e.SupplierDeliveryNoteNumber=r.SupplierDeliveryNoteNumber?.Trim();e.ReceiptDate=r.ReceiptDate;e.Notes=r.Notes?.Trim();
        db.GoodsReceiptLines.RemoveRange(e.Lines);db.Set<GoodsReceiptPurchaseOrder>().RemoveRange(e.PurchaseOrders);e.Lines=[];e.PurchaseOrders=r.PurchaseOrderIds.Where(x=>x>0).Distinct().Select(x=>new GoodsReceiptPurchaseOrder{PurchaseOrderId=x}).ToList();var n=0;foreach(var l in r.Lines){if(l.ReceivedQuantity<=0||l.AcceptedQuantity+l.RejectedQuantity!=l.ReceivedQuantity)return BadRequest(ApiResponse<object>.Error("Accepted and rejected quantities must equal received quantity.",400));var line=new GoodsReceiptLine{LineNumber=++n,PurchaseOrderLineId=l.PurchaseOrderLineId,ProductId=l.ProductId,UnitId=l.UnitId,StorageLocationId=l.StorageLocationId,InventoryStatusId=l.InventoryStatusId,ExpectedQuantity=l.ExpectedQuantity,ReceivedQuantity=l.ReceivedQuantity,AcceptedQuantity=l.AcceptedQuantity,RejectedQuantity=l.RejectedQuantity,UnitCost=l.UnitCost,LotNumber=l.LotNumber?.Trim(),ManufactureDate=l.ManufactureDate,ExpiryDate=l.ExpiryDate,Notes=l.Notes?.Trim()};foreach(var serial in (l.SerialNumbers??[]).Select(x=>x.Trim()).Where(x=>x.Length>0).Distinct())line.Serials.Add(new(){SerialNumber=serial});e.Lines.Add(line);}await db.SaveChangesAsync(ct);return Ok(ApiResponse<object>.Ok(new{e.Id}));
    }
    [HttpDelete]
    public async Task<IActionResult>Delete(long id,CancellationToken ct)
    {
        var entity=await db.GoodsReceipts.Include(x=>x.Lines).ThenInclude(x=>x.Serials).Include(x=>x.PurchaseOrders).FirstOrDefaultAsync(x=>x.Id==id,ct);
        if(entity is null)return NotFound(ApiResponse<object>.Error("Goods receipt not found.",404));
        if(entity.Status is not (GoodsReceiptStatus.Draft or GoodsReceiptStatus.Registered))return Conflict(ApiResponse<object>.Error("Only unposted goods receipts can be deleted.",409));
        db.GoodsReceipts.Remove(entity);await db.SaveChangesAsync(ct);return Ok(ApiResponse<object>.Ok(new{entity.Id}));
    }
}
