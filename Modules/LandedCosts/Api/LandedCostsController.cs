using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using verii_metivon_api.Core.Auth;
using verii_metivon_api.Core.Paging;
using verii_metivon_api.Core.Persistence;
using verii_metivon_api.Modules.LandedCosts.Application.Services;
using verii_metivon_api.Modules.LandedCosts.Domain.Entities;

namespace verii_metivon_api.Modules.LandedCosts.Api;

[ApiController, Authorize, Route("api/import-dossiers")]
public sealed class LandedCostsController(ILandedCostService service, MetivonDbContext db, IWebHostEnvironment environment) : ControllerBase
{
    [HttpGet] public async Task<IActionResult> List([FromQuery] ImportDossierQuery query, CancellationToken ct) { var r=await service.GetPagedAsync(query,ct);return StatusCode(r.StatusCode,r); }
    [HttpPost] public async Task<IActionResult> Create(CreateImportDossierRequest request,CancellationToken ct){var r=await service.CreateAsync(request,ct);return StatusCode(r.StatusCode,r);}
    [HttpPost("{id:long}/costs")] public async Task<IActionResult> Cost(long id,AddDossierCostRequest request,CancellationToken ct){var r=await service.AddCostAsync(id,request,ct);return StatusCode(r.StatusCode,r);}
    [HttpPost("{id:long}/allocate")] public async Task<IActionResult> Allocate(long id,CancellationToken ct){var r=await service.AllocateAsync(id,ct);return StatusCode(r.StatusCode,r);}
    [HttpPost("{id:long}/finalize")] public async Task<IActionResult> Finalize(long id,FinalizeDossierRequest request,CancellationToken ct){var r=await service.FinalizeAsync(id,request,ct);return StatusCode(r.StatusCode,r);}
    [HttpGet("{id:long}")]
    public async Task<IActionResult> Detail(long id,CancellationToken ct)
    {
        var dossier=await db.ImportDossiers.AsNoTracking().Where(x=>x.Id==id).Select(x=>new{x.Id,x.DossierNumber,Status=x.Status.ToString(),x.BranchId,x.SupplierId,Supplier=x.Supplier.Name,x.CurrencyId,x.CurrencyCode,CurrencySymbol=x.Currency.Symbol,x.IncotermCode,x.CustomsDeclarationNumber,x.CustomsDeclarationDate,x.TransactionExchangeRate,x.CustomsExchangeRate,x.CostingExchangeRate,x.OpenDate,x.EstimatedArrivalDate,x.ActualArrivalDate,x.FinalizedAt,x.AllocationRevision,x.Notes}).FirstOrDefaultAsync(ct);
        if(dossier is null)return NotFound(ApiResponse<object>.Error("Dossier not found.",404));
        var lines=await db.ImportDossierLines.AsNoTracking().Where(x=>x.ImportDossierId==id).OrderBy(x=>x.LineNumber).Select(x=>new{x.Id,x.LineNumber,x.ProductId,ProductCode=x.Product.Code,ProductName=x.Product.Name,x.Quantity,x.NetWeight,x.GrossWeight,x.Volume,x.ForeignUnitPrice,x.ForeignGoodsAmount,x.GoodsAmountLocal,x.AllocatedCostLocal,x.FinalUnitCostLocal,x.PurchaseOrderLineId,x.GoodsReceiptLineId,x.ReceiptTransactionId}).ToListAsync(ct);
        var tradeDossierId=await db.ImportDossiers.AsNoTracking().Where(x=>x.Id==id).Select(x=>x.TradeDossierId).FirstAsync(ct);
        var goodsReceipts=tradeDossierId.HasValue
            ? await db.GoodsReceipts.AsNoTracking().Where(x=>x.TradeDossierId==tradeDossierId.Value).OrderByDescending(x=>x.ReceiptDate).ThenByDescending(x=>x.Id).Select(x=>new
            {
                x.Id,x.ReceiptNumber,ReceiptType=x.ReceiptType.ToString(),Status=x.Status.ToString(),x.ReceiptDate,x.CreatedAt,x.PostedAt,
                CreatedBy=x.CreatedBy.HasValue?db.Users.Where(u=>u.Id==x.CreatedBy.Value).Select(u=>u.Detail!=null&&((u.Detail.FirstName??"")+" "+(u.Detail.LastName??"")).Trim()!=""?((u.Detail.FirstName??"")+" "+(u.Detail.LastName??"")).Trim():u.Username).FirstOrDefault():null,
                Warehouse=x.Warehouse.Code,
                Lines=x.Lines.OrderBy(l=>l.LineNumber).Select(l=>new{l.Id,l.LineNumber,l.ProductId,ProductCode=l.Product.Code,ProductName=l.Product.Name,Unit=l.Unit.Code,l.ExpectedQuantity,l.ReceivedQuantity,l.AcceptedQuantity,l.RejectedQuantity,l.UnitCost,l.LotNumber,l.ManufactureDate,l.ExpiryDate,l.CreatedAt}).ToList()
            }).ToListAsync(ct)
            : [];
        var costs=await db.ImportDossierCosts.AsNoTracking().Where(x=>x.ImportDossierId==id).OrderBy(x=>x.Id).Select(x=>new{x.Id,CostType=x.LandedCostType.Name,SourceType=x.SourceType.ToString(),AmountType=x.AmountType.ToString(),AllocationMethod=x.AllocationMethod.ToString(),x.SupplierId,x.InvoiceNumber,x.InvoiceDate,x.PaymentReference,x.PaymentDate,x.CurrencyId,x.CurrencyCode,CurrencySymbol=x.Currency.Symbol,x.ForeignAmount,x.OriginalExchangeRate,AppliedExchangeRate=x.ExchangeRate,x.ExchangeRateDate,x.ExchangeRateSource,x.LocalAmount,x.Description}).ToListAsync(ct);
        var documents=await db.ImportDossierDocuments.AsNoTracking().Where(x=>x.ImportDossierId==id).OrderByDescending(x=>x.CreatedAt).Select(x=>new{x.Id,DocumentType=x.DocumentType.ToString(),x.DocumentNumber,x.DocumentDate,x.OriginalFileName,x.ContentType,x.FileSize,x.ImportDossierCostId,x.Notes,x.CreatedAt,DownloadUrl=$"/api/import-dossiers/{id}/documents/{x.Id}"}).ToListAsync(ct);
        var timeline=documents.Select(x=>new{At=x.CreatedAt,Kind="Document",Title=x.DocumentType,Detail=x.OriginalFileName}).Concat(costs.Select(x=>new{At=DateTime.MinValue,Kind="Cost",Title=x.CostType,Detail=$"{x.LocalAmount:N2} TRY Â· {x.SourceType}"})).OrderByDescending(x=>x.At).ToList();
        return Ok(ApiResponse<object>.Ok(new{dossier,lines,goodsReceipts,costs,documents,timeline,totals=new{Goods=lines.Sum(x=>x.GoodsAmountLocal),Costs=costs.Sum(x=>x.LocalAmount),Final=lines.Sum(x=>x.GoodsAmountLocal+x.AllocatedCostLocal)}}));
    }
    [HttpPut("{id:long}/customs-declaration")]
    public async Task<IActionResult> Customs(long id,SaveCustomsDeclarationRequest request,CancellationToken ct){var d=await db.ImportDossiers.FirstOrDefaultAsync(x=>x.Id==id,ct);if(d is null)return NotFound(ApiResponse<object>.Error("Dossier not found.",404));if(d.Status>=ImportDossierStatus.Finalized)return Conflict(ApiResponse<object>.Error("Finalized dossier cannot be changed.",409));d.CustomsDeclarationNumber=request.Number.Trim().ToUpperInvariant();d.CustomsDeclarationDate=request.Date;d.CustomsExchangeRate=request.ExchangeRate;d.ActualArrivalDate=request.ActualArrivalDate;d.Status=ImportDossierStatus.ReceivedProvisionally;await db.SaveChangesAsync(ct);return Ok(ApiResponse<object>.Ok(new{d.Id,d.Status}));}
    [HttpPost("{id:long}/documents"),RequestSizeLimit(10_485_760)]
    public async Task<IActionResult> Upload(long id,[FromForm]UploadImportDocumentRequest request,CancellationToken ct){if(!await db.ImportDossiers.AnyAsync(x=>x.Id==id,ct))return NotFound(ApiResponse<object>.Error("Dossier not found.",404));if(request.File is null||request.File.Length==0||request.File.Length>10_485_760)return BadRequest(ApiResponse<object>.Error("A file up to 10 MB is required.",400));var allowed=new Dictionary<string,string>(StringComparer.OrdinalIgnoreCase){{".pdf","application/pdf"},{".jpg","image/jpeg"},{".jpeg","image/jpeg"},{".png","image/png"}};var ext=Path.GetExtension(request.File.FileName);if(!allowed.TryGetValue(ext,out var expected)||!string.Equals(request.File.ContentType,expected,StringComparison.OrdinalIgnoreCase))return BadRequest(ApiResponse<object>.Error("Only PDF, JPG and PNG files are allowed.",400));var folder=Path.Combine(environment.ContentRootPath,"App_Data","import-dossiers",id.ToString());Directory.CreateDirectory(folder);var stored=$"{Guid.NewGuid():N}{ext.ToLowerInvariant()}";var path=Path.Combine(folder,stored);await using(var output=System.IO.File.Create(path)){await request.File.CopyToAsync(output,ct);}await using var input=System.IO.File.OpenRead(path);var hash=Convert.ToHexString(await SHA256.HashDataAsync(input,ct));var entity=new ImportDossierDocument{ImportDossierId=id,ImportDossierCostId=request.ImportDossierCostId,DocumentType=request.DocumentType,DocumentNumber=request.DocumentNumber?.Trim()??string.Empty,DocumentDate=request.DocumentDate,OriginalFileName=Path.GetFileName(request.File.FileName),StoredFileName=stored,ContentType=expected,FileSize=request.File.Length,Sha256=hash,Notes=request.Notes};db.ImportDossierDocuments.Add(entity);await db.SaveChangesAsync(ct);return Ok(ApiResponse<object>.Ok(new{entity.Id,entity.OriginalFileName}));}
    [HttpGet("{id:long}/documents/{documentId:long}")]
    public async Task<IActionResult> Download(long id,long documentId,CancellationToken ct){var d=await db.ImportDossierDocuments.AsNoTracking().FirstOrDefaultAsync(x=>x.Id==documentId&&x.ImportDossierId==id,ct);if(d is null)return NotFound();var path=Path.Combine(environment.ContentRootPath,"App_Data","import-dossiers",id.ToString(),d.StoredFileName);if(!System.IO.File.Exists(path))return NotFound();return PhysicalFile(path,d.ContentType,d.OriginalFileName);}
    [HttpGet("cost-types")]
    public async Task<IActionResult> CostTypes([FromQuery] LandedCostTypeQuery query,CancellationToken ct)
    {
        var source=db.LandedCostTypes.AsNoTracking();
        if(!string.IsNullOrWhiteSpace(query.Search)){var search=query.Search.Trim();source=source.Where(x=>x.Code.Contains(search)||x.Name.Contains(search));}
        source=source.ApplyPagedFilters(query);
        var total=await source.CountAsync(ct);
        var rows=await source.ApplyPagedSort(query,nameof(LandedCostType.DisplayOrder),aliases:new Dictionary<string,string>(StringComparer.OrdinalIgnoreCase)
            {{"allocationMethod",nameof(LandedCostType.DefaultAllocationMethod)}}).ThenBy(x=>x.Id).Skip((query.NormalizedPageNumber-1)*query.NormalizedPageSize).Take(query.NormalizedPageSize)
            .Select(x=>new LandedCostTypeRow(x.Id,x.Code,x.Name,x.DefaultAllocationMethod.ToString(),x.IncludeInCustomsValue,x.CapitalizeToInventory,x.ClearingAccountId,x.VarianceAccountId,x.IsDefault,x.IsActive)).ToListAsync(ct);
        return Ok(ApiResponse<PagedResult<LandedCostTypeRow>>.Ok(new(rows,query.NormalizedPageNumber,query.NormalizedPageSize,total)));
    }
    [HttpPost("cost-types")] public async Task<IActionResult> Type(SaveLandedCostTypeRequest request,CancellationToken ct){var r=await service.SaveTypeAsync(request,ct);return StatusCode(r.StatusCode,r);}
    [HttpGet("cost-types/{id:long}")]
    public async Task<IActionResult> CostType(long id,CancellationToken ct)
    {
        var row=await db.LandedCostTypes.AsNoTracking().Where(x=>x.Id==id).Select(x=>new{x.Id,x.Code,x.Name,x.Description,DefaultAllocationMethod=(int)x.DefaultAllocationMethod,x.IncludeInCustomsValue,x.CapitalizeToInventory,x.ClearingAccountId,x.VarianceAccountId,x.IsActive,x.IsDefault,x.DisplayOrder}).FirstOrDefaultAsync(ct);
        return row is null?NotFound(ApiResponse<object>.Error("Cost type not found.",404)):Ok(ApiResponse<object>.Ok(row));
    }
    [HttpPut("cost-types/{id:long}")]
    public async Task<IActionResult> UpdateCostType(long id,SaveLandedCostTypeRequest request,CancellationToken ct)
    {
        var entity=await db.LandedCostTypes.FirstOrDefaultAsync(x=>x.Id==id,ct);
        if(entity is null)return NotFound(ApiResponse<object>.Error("Cost type not found.",404));
        var code=request.Code.Trim().ToUpperInvariant();
        if(string.IsNullOrWhiteSpace(code)||string.IsNullOrWhiteSpace(request.Name))return BadRequest(ApiResponse<object>.Error("Code and name are required.",400));
        if(await db.LandedCostTypes.AnyAsync(x=>x.Id!=id&&x.Code==code,ct))return Conflict(ApiResponse<object>.Error("Cost type code exists.",409));
        if(request.IsDefault){var defaults=await db.LandedCostTypes.Where(x=>x.Id!=id&&x.IsDefault).ToListAsync(ct);defaults.ForEach(x=>x.IsDefault=false);}
        entity.Code=code;entity.Name=request.Name.Trim();entity.Description=request.Description?.Trim();entity.DefaultAllocationMethod=request.DefaultAllocationMethod;entity.IncludeInCustomsValue=request.IncludeInCustomsValue;entity.CapitalizeToInventory=request.CapitalizeToInventory;entity.ClearingAccountId=request.ClearingAccountId;entity.VarianceAccountId=request.VarianceAccountId;entity.IsActive=request.IsActive;entity.IsDefault=request.IsDefault;entity.DisplayOrder=request.DisplayOrder;entity.UpdatedAt=DateTime.UtcNow;
        await db.SaveChangesAsync(ct);return Ok(ApiResponse<object>.Ok(new{entity.Id}));
    }
    [HttpDelete("cost-types/{id:long}")]
    public async Task<IActionResult> DeleteCostType(long id,CancellationToken ct)
    {
        var entity=await db.LandedCostTypes.FirstOrDefaultAsync(x=>x.Id==id,ct);
        if(entity is null)return NotFound(ApiResponse<object>.Error("Cost type not found.",404));
        if(entity.IsDefault)return Conflict(ApiResponse<object>.Error("Default cost type cannot be deleted.",409));
        if(await db.ImportDossierCosts.AnyAsync(x=>x.LandedCostTypeId==id,ct))return Conflict(ApiResponse<object>.Error("Cost type is in use and cannot be deleted.",409));
        db.LandedCostTypes.Remove(entity);await db.SaveChangesAsync(ct);return Ok(ApiResponse<object>.Ok(new{id}));
    }
}

public sealed class LandedCostTypeQuery : PagedQuery { }
public sealed record LandedCostTypeRow(long Id,string Code,string Name,string AllocationMethod,bool IncludeInCustomsValue,bool CapitalizeToInventory,long? ClearingAccountId,long? VarianceAccountId,bool IsDefault,bool IsActive);
public sealed record SaveCustomsDeclarationRequest(string Number,DateOnly Date,decimal ExchangeRate,DateOnly? ActualArrivalDate);
public sealed class UploadImportDocumentRequest{public ImportDocumentType DocumentType{get;set;}public string?DocumentNumber{get;set;}public DateOnly?DocumentDate{get;set;}public long?ImportDossierCostId{get;set;}public string?Notes{get;set;}public IFormFile?File{get;set;}}

