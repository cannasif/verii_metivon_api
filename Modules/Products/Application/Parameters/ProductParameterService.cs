using System.Data;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using verii_metivon_api.Core.Persistence;
using verii_metivon_api.Modules.Parameters.Domain.Entities;
using verii_metivon_api.Modules.Products.Domain.Entities;
using verii_metivon_api.Modules.Products.Domain.Enums;

namespace verii_metivon_api.Modules.Products.Application.Parameters;

public sealed record ProductParametersDto(long? BranchId,bool IsAutomatic,bool AllowManual,string Format,long NextNumber,int IncrementBy,long MinimumNumber,long MaximumNumber,bool IsContinuous,bool ForceUppercase,bool TrimWhitespace,long? DefaultProductCategoryId,long? DefaultProductGroupId,long? DefaultBrandId,long? DefaultBaseUnitId,long? DefaultPurchaseTaxGroupId,long? DefaultSalesTaxGroupId,int DefaultProductType,int DefaultLifecycleStatus,int DefaultTrackingType,int DefaultValuationMethod,int DefaultProcurementType,int? DefaultShelfLifeDays,bool DefaultPurchasable,bool DefaultSellable,bool DefaultInventoryTracked,bool RequireCountryOfOrigin,bool RequireCustomsTariffCode,bool RequireNetWeight,bool RequireGrossWeight,bool RequireShelfLife,decimal MinimumOrderQuantity,decimal MaximumOrderQuantity,decimal OrderMultiple,int DefaultLeadTimeDays,int DefaultBarcodeType,int MinimumRemainingShelfLifeDays,bool UseFefo,string Preview);
public sealed record SaveProductParametersRequest(long? BranchId,bool IsAutomatic,bool AllowManual,string Format,long NextNumber,int IncrementBy,long MinimumNumber,long MaximumNumber,bool IsContinuous,bool ForceUppercase,bool TrimWhitespace,long? DefaultProductCategoryId,long? DefaultProductGroupId,long? DefaultBrandId,long? DefaultBaseUnitId,long? DefaultPurchaseTaxGroupId,long? DefaultSalesTaxGroupId,int DefaultProductType,int DefaultLifecycleStatus,int DefaultTrackingType,int DefaultValuationMethod,int DefaultProcurementType,int? DefaultShelfLifeDays,bool DefaultPurchasable,bool DefaultSellable,bool DefaultInventoryTracked,bool RequireCountryOfOrigin,bool RequireCustomsTariffCode,bool RequireNetWeight,bool RequireGrossWeight,bool RequireShelfLife,decimal MinimumOrderQuantity,decimal MaximumOrderQuantity,decimal OrderMultiple,int DefaultLeadTimeDays,int DefaultBarcodeType,int MinimumRemainingShelfLifeDays,bool UseFefo);

public interface IProductParameterService
{
    Task<ProductParametersDto> GetAsync(long? branchId,CancellationToken ct);
    Task<ProductParametersDto> SaveAsync(SaveProductParametersRequest request,CancellationToken ct);
    Task<string?> ResolveCodeAsync(string? requestedCode,long? branchId,ProductType productType,CancellationToken ct);
}

public sealed class ProductParameterService(MetivonDbContext db):IProductParameterService
{
    private const string Module="Products";
    private const string Reference="ProductCode";
    private static readonly Regex NumberToken=new(@"\{NUMBER(?::(?<length>\d{1,2}))?\}",RegexOptions.Compiled|RegexOptions.IgnoreCase);

    public async Task<ProductParametersDto> GetAsync(long? branchId,CancellationToken ct)
    {
        var sequence=await ResolveSequence(branchId,ct);var settings=await ResolveSettings(branchId,ct);
        var branchCode=branchId.HasValue?await db.Branches.AsNoTracking().Where(x=>x.Id==branchId).Select(x=>x.Code).FirstOrDefaultAsync(ct)??"0":"0";
        return Map(sequence,settings,branchCode);
    }

    public async Task<ProductParametersDto> SaveAsync(SaveProductParametersRequest request,CancellationToken ct)
    {
        Validate(request);await ValidateReferences(request,ct);
        await using var transaction=await db.Database.BeginTransactionAsync(IsolationLevel.Serializable,ct);
        var sequence=await db.NumberSequences.FirstOrDefaultAsync(x=>x.Module==Module&&x.Reference==Reference&&x.BranchId==request.BranchId,ct);
        if(sequence is null){sequence=new NumberSequence{Module=Module,Reference=Reference,BranchId=request.BranchId};db.Add(sequence);}
        sequence.Format=request.Format.Trim();sequence.CurrentNumber=request.NextNumber;sequence.IncrementBy=request.IncrementBy;sequence.MinimumNumber=request.MinimumNumber;sequence.MaximumNumber=request.MaximumNumber;sequence.IsAutomatic=request.IsAutomatic;sequence.AllowManual=request.AllowManual;sequence.IsContinuous=request.IsContinuous;sequence.IsActive=true;

        var settings=await db.ProductParameterSettings.FirstOrDefaultAsync(x=>x.BranchId==request.BranchId,ct);
        if(settings is null){settings=new ProductParameterSettings{BranchId=request.BranchId};db.Add(settings);}
        Apply(settings,request);
        await db.SaveChangesAsync(ct);await transaction.CommitAsync(ct);return await GetAsync(request.BranchId,ct);
    }

    public async Task<string?> ResolveCodeAsync(string? requestedCode,long? branchId,ProductType productType,CancellationToken ct)
    {
        var sequence=await ResolveSequence(branchId,ct);var settings=await ResolveSettings(branchId,ct);var manual=Normalize(requestedCode,settings);
        if(!string.IsNullOrWhiteSpace(manual)){if(sequence.IsAutomatic&&!sequence.AllowManual)throw new InvalidOperationException("Manual product code is not allowed by product parameters.");return manual;}
        if(!sequence.IsAutomatic)return null;
        var branchCode=branchId.HasValue?await db.Branches.AsNoTracking().Where(x=>x.Id==branchId).Select(x=>x.Code).FirstOrDefaultAsync(ct)??"0":"0";
        return Normalize(Format(sequence.Format,await Consume(sequence,ct),branchCode,productType.ToString().ToUpperInvariant()),settings);
    }

    private async Task<NumberSequence> ResolveSequence(long? branchId,CancellationToken ct)=>await db.NumberSequences.AsNoTracking().Where(x=>x.Module==Module&&x.Reference==Reference&&x.IsActive&&(x.BranchId==branchId||x.BranchId==null)).OrderByDescending(x=>x.BranchId.HasValue).FirstOrDefaultAsync(ct)??new NumberSequence{Module=Module,Reference=Reference,BranchId=branchId,Format="STK-{NUMBER:6}",CurrentNumber=1,IncrementBy=1,MinimumNumber=1,MaximumNumber=999999,IsAutomatic=true,AllowManual=true};
    private async Task<ProductParameterSettings> ResolveSettings(long? branchId,CancellationToken ct)=>await db.ProductParameterSettings.AsNoTracking().Where(x=>x.BranchId==branchId||x.BranchId==null).OrderByDescending(x=>x.BranchId.HasValue).FirstOrDefaultAsync(ct)??new ProductParameterSettings{BranchId=branchId};

    private static ProductParametersDto Map(NumberSequence s,ProductParameterSettings p,string branch)=>new(s.BranchId,s.IsAutomatic,s.AllowManual,s.Format,s.CurrentNumber,s.IncrementBy,s.MinimumNumber,s.MaximumNumber,s.IsContinuous,p.ForceUppercase,p.TrimWhitespace,p.DefaultProductCategoryId,p.DefaultProductGroupId,p.DefaultBrandId,p.DefaultBaseUnitId,p.DefaultPurchaseTaxGroupId,p.DefaultSalesTaxGroupId,(int)p.DefaultProductType,(int)p.DefaultLifecycleStatus,(int)p.DefaultTrackingType,(int)p.DefaultValuationMethod,(int)p.DefaultProcurementType,p.DefaultShelfLifeDays,p.DefaultPurchasable,p.DefaultSellable,p.DefaultInventoryTracked,p.RequireCountryOfOrigin,p.RequireCustomsTariffCode,p.RequireNetWeight,p.RequireGrossWeight,p.RequireShelfLife,p.MinimumOrderQuantity,p.MaximumOrderQuantity,p.OrderMultiple,p.DefaultLeadTimeDays,(int)p.DefaultBarcodeType,p.MinimumRemainingShelfLifeDays,p.UseFefo,Format(s.Format,s.CurrentNumber,branch,"GOODS"));

    private static void Apply(ProductParameterSettings p,SaveProductParametersRequest r)
    {
        p.ForceUppercase=r.ForceUppercase;p.TrimWhitespace=r.TrimWhitespace;p.DefaultProductCategoryId=r.DefaultProductCategoryId;p.DefaultProductGroupId=r.DefaultProductGroupId;p.DefaultBrandId=r.DefaultBrandId;p.DefaultBaseUnitId=r.DefaultBaseUnitId;p.DefaultPurchaseTaxGroupId=r.DefaultPurchaseTaxGroupId;p.DefaultSalesTaxGroupId=r.DefaultSalesTaxGroupId;p.DefaultProductType=(ProductType)r.DefaultProductType;p.DefaultLifecycleStatus=(ProductLifecycleStatus)r.DefaultLifecycleStatus;p.DefaultTrackingType=(InventoryTrackingType)r.DefaultTrackingType;p.DefaultValuationMethod=(InventoryValuationMethod)r.DefaultValuationMethod;p.DefaultProcurementType=(ProcurementType)r.DefaultProcurementType;p.DefaultShelfLifeDays=r.DefaultShelfLifeDays;p.DefaultPurchasable=r.DefaultPurchasable;p.DefaultSellable=r.DefaultSellable;p.DefaultInventoryTracked=r.DefaultInventoryTracked;p.RequireCountryOfOrigin=r.RequireCountryOfOrigin;p.RequireCustomsTariffCode=r.RequireCustomsTariffCode;p.RequireNetWeight=r.RequireNetWeight;p.RequireGrossWeight=r.RequireGrossWeight;p.RequireShelfLife=r.RequireShelfLife;p.MinimumOrderQuantity=r.MinimumOrderQuantity;p.MaximumOrderQuantity=r.MaximumOrderQuantity;p.OrderMultiple=r.OrderMultiple;p.DefaultLeadTimeDays=r.DefaultLeadTimeDays;p.DefaultBarcodeType=(BarcodeType)r.DefaultBarcodeType;p.MinimumRemainingShelfLifeDays=r.MinimumRemainingShelfLifeDays;p.UseFefo=r.UseFefo;
    }

    private async Task ValidateReferences(SaveProductParametersRequest r,CancellationToken ct)
    {
        if(r.BranchId.HasValue&&!await db.Branches.AnyAsync(x=>x.Id==r.BranchId&&x.IsActive,ct))throw new ArgumentException("Selected branch is not active.");
        if(r.DefaultProductCategoryId.HasValue&&!await db.ProductCategories.AnyAsync(x=>x.Id==r.DefaultProductCategoryId&&x.IsActive,ct))throw new ArgumentException("Selected default product category is not active.");
        if(r.DefaultProductGroupId.HasValue&&!await db.ProductGroups.AnyAsync(x=>x.Id==r.DefaultProductGroupId&&x.IsActive,ct))throw new ArgumentException("Selected default product group is not active.");
        if(r.DefaultBrandId.HasValue&&!await db.Brands.AnyAsync(x=>x.Id==r.DefaultBrandId&&x.IsActive,ct))throw new ArgumentException("Selected default brand is not active.");
        if(r.DefaultBaseUnitId.HasValue&&!await db.Units.AnyAsync(x=>x.Id==r.DefaultBaseUnitId&&x.IsActive,ct))throw new ArgumentException("Selected default unit is not active.");
        if(r.DefaultPurchaseTaxGroupId.HasValue&&!await db.TaxGroups.AnyAsync(x=>x.Id==r.DefaultPurchaseTaxGroupId&&x.IsActive,ct))throw new ArgumentException("Selected purchase tax group is not active.");
        if(r.DefaultSalesTaxGroupId.HasValue&&!await db.TaxGroups.AnyAsync(x=>x.Id==r.DefaultSalesTaxGroupId&&x.IsActive,ct))throw new ArgumentException("Selected sales tax group is not active.");
    }

    private static void Validate(SaveProductParametersRequest r)
    {
        if(!NumberToken.IsMatch(r.Format))throw new ArgumentException("Format must contain a {NUMBER} or {NUMBER:n} token.");if(r.Format.Length>120)throw new ArgumentException("Format cannot exceed 120 characters.");
        if(r.IncrementBy<1||r.MinimumNumber<0||r.NextNumber<r.MinimumNumber||r.MaximumNumber<r.NextNumber)throw new ArgumentException("Number range values are invalid.");
        if(r.DefaultShelfLifeDays is <0||r.MinimumOrderQuantity<0||r.MaximumOrderQuantity<0||r.OrderMultiple<=0||r.DefaultLeadTimeDays<0||r.MinimumRemainingShelfLifeDays<0)throw new ArgumentException("One or more numeric product parameters are invalid.");
        if(r.MaximumOrderQuantity>0&&r.MaximumOrderQuantity<r.MinimumOrderQuantity)throw new ArgumentException("Maximum order quantity cannot be lower than minimum order quantity.");
        if(!Enum.IsDefined(typeof(ProductType),r.DefaultProductType)||!Enum.IsDefined(typeof(ProductLifecycleStatus),r.DefaultLifecycleStatus)||!Enum.IsDefined(typeof(InventoryTrackingType),r.DefaultTrackingType)||!Enum.IsDefined(typeof(InventoryValuationMethod),r.DefaultValuationMethod)||!Enum.IsDefined(typeof(ProcurementType),r.DefaultProcurementType)||!Enum.IsDefined(typeof(BarcodeType),r.DefaultBarcodeType))throw new ArgumentException("One or more product default selections are invalid.");
        if(Format(r.Format,r.NextNumber,"0","GOODS").Length>80)throw new ArgumentException("Generated product code cannot exceed 80 characters.");
    }

    private async Task<long> Consume(NumberSequence s,CancellationToken ct){if(s.Id==0){db.Add(s);await db.SaveChangesAsync(ct);}if(s.CurrentNumber>s.MaximumNumber)throw new InvalidOperationException("Product number sequence is exhausted.");var c=db.Database.GetDbConnection();if(c.State!=ConnectionState.Open)await c.OpenAsync(ct);await using var command=c.CreateCommand();command.Transaction=db.Database.CurrentTransaction?.GetDbTransaction();command.CommandText="UPDATE RII_NUMBER_SEQUENCES WITH (UPDLOCK, ROWLOCK) SET CurrentNumber = CurrentNumber + IncrementBy, UpdatedAt = SYSUTCDATETIME() OUTPUT DELETED.CurrentNumber WHERE Id = @id AND IsActive = 1 AND CurrentNumber <= MaximumNumber";var p=command.CreateParameter();p.ParameterName="@id";p.Value=s.Id;command.Parameters.Add(p);var result=await command.ExecuteScalarAsync(ct);return result is null||result==DBNull.Value?throw new InvalidOperationException("Product number sequence could not be consumed."):Convert.ToInt64(result);}
    private static string? Normalize(string? value,ProductParameterSettings p){if(string.IsNullOrWhiteSpace(value))return null;var result=p.TrimWhitespace?value.Trim():value;return p.ForceUppercase?result.ToUpperInvariant():result;}
    private static string Format(string format,long number,string branch,string type){var result=format.Replace("{BRANCH}",branch,StringComparison.OrdinalIgnoreCase).Replace("{TYPE}",type,StringComparison.OrdinalIgnoreCase).Replace("{YYYY}",DateTime.UtcNow.ToString("yyyy"),StringComparison.OrdinalIgnoreCase).Replace("{YY}",DateTime.UtcNow.ToString("yy"),StringComparison.OrdinalIgnoreCase).Replace("{MM}",DateTime.UtcNow.ToString("MM"),StringComparison.OrdinalIgnoreCase);return NumberToken.Replace(result,m=>number.ToString().PadLeft(Math.Clamp(int.TryParse(m.Groups["length"].Value,out var length)?length:1,1,18),'0'));}
}
