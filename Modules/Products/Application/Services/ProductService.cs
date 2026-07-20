using Microsoft.EntityFrameworkCore;
using verii_metivon_api.Core.Auth;
using verii_metivon_api.Core.Domain;
using verii_metivon_api.Core.Paging;
using verii_metivon_api.Core.Repositories;
using verii_metivon_api.Core.UnitOfWork;
using verii_metivon_api.Modules.Products.Domain.Entities;
using verii_metivon_api.Modules.Products.Domain.Enums;
using verii_metivon_api.Modules.Products.Localization;
using verii_metivon_api.Modules.Products.Application.Parameters;
using UnitEntity = verii_metivon_api.Modules.Products.Domain.Entities.Unit;

namespace verii_metivon_api.Modules.Products.Application.Services;

public sealed class ProductService(IUnitOfWork unitOfWork, IProductParameterService parameters) : IProductService
{
    private static readonly IReadOnlyDictionary<string, string> ListFilterAliases =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["category"] = "ProductCategory.Name",
            ["group"] = "ProductGroup.Name",
            ["brand"] = "Brand.Name",
            ["baseUnit"] = "BaseUnit.Symbol"
        };

    public async Task<ApiResponse<PagedResult<ProductListItem>>> GetPagedAsync(ProductListQuery request, string? culture, CancellationToken ct)
    {
        var query = unitOfWork.Repository<Product>().Query();
        if (request.CategoryId.HasValue) query = query.Where(x => x.ProductCategoryId == request.CategoryId);
        if (request.GroupId.HasValue) query = query.Where(x => x.ProductGroupId == request.GroupId);
        if (request.BrandId.HasValue) query = query.Where(x => x.BrandId == request.BrandId);
        if (request.ProductType.HasValue) query = query.Where(x => x.ProductType == request.ProductType);
        if (request.TrackingType.HasValue) query = query.Where(x => x.TrackingType == request.TrackingType);
        if (request.LifecycleStatus.HasValue) query = query.Where(x => x.LifecycleStatus == request.LifecycleStatus);
        if (request.IsActive.HasValue) query = query.Where(x => x.IsActive == request.IsActive);
        var search = request.Search?.Trim();
        if (!string.IsNullOrWhiteSpace(search)) query = query.Where(x => x.Code.Contains(search) || x.Name.Contains(search) ||
            (x.SearchName != null && x.SearchName.Contains(search)) || (x.ManufacturerCode != null && x.ManufacturerCode.Contains(search)) ||
            x.Barcodes.Any(b => b.Barcode.Contains(search)));
        query = query.ApplyPagedFilters(request, ListFilterAliases);
        query = ApplySort(query, request.SortBy, request.IsDescending);
        var total = await query.CountAsync(ct);
        var items = await query.Skip((request.NormalizedPageNumber - 1) * request.NormalizedPageSize).Take(request.NormalizedPageSize)
            .Select(x => new ProductListItem(x.Id, x.Code, x.Name, x.ProductCategory.Name, x.ProductGroup.Name, x.Brand != null ? x.Brand.Name : null,
                x.BaseUnit.Symbol, x.ProductType, x.TrackingType, x.LifecycleStatus, x.IsActive)).ToListAsync(ct);
        return ApiResponse<PagedResult<ProductListItem>>.Ok(new(items, request.NormalizedPageNumber, request.NormalizedPageSize, total), ProductMessages.Get("Retrieved", culture));
    }

    public async Task<ApiResponse<ProductDefinitions>> GetDefinitionsAsync(string? culture, CancellationToken ct)
    {
        static async Task<IReadOnlyList<DefinitionItem>> Read<T>(IGenericRepository<T> repository, CancellationToken token) where T : DefinitionEntity =>
            await repository.Query().Where(x => x.IsActive).OrderBy(x => x.DisplayOrder).ThenBy(x => x.Name).Select(x => new DefinitionItem(x.Id, x.Code, x.Name, x.IsDefault)).ToListAsync(token);
        var definitions = new ProductDefinitions(
            await Read(unitOfWork.Repository<ProductCategory>(), ct), await Read(unitOfWork.Repository<ProductGroup>(), ct),
            await Read(unitOfWork.Repository<Brand>(), ct), await Read(unitOfWork.Repository<UnitCategory>(), ct),
            await Read(unitOfWork.Repository<UnitEntity>(), ct), await Read(unitOfWork.Repository<PackageType>(), ct), await Read(unitOfWork.TaxGroups, ct));
        return ApiResponse<ProductDefinitions>.Ok(definitions, ProductMessages.Get("DefinitionsRetrieved", culture));
    }

    public async Task<ApiResponse<object>> CreateAsync(SaveProductRequest request, string? culture, CancellationToken ct)
    {
        var settings = await parameters.GetAsync(request.BranchId, ct);
        var productType = Enum.IsDefined(request.ProductType) && (int)request.ProductType > 0 ? request.ProductType : (ProductType)settings.DefaultProductType;
        var effective = request with
        {
            ProductType = productType,
            LifecycleStatus = Enum.IsDefined(request.LifecycleStatus) && (int)request.LifecycleStatus > 0 ? request.LifecycleStatus : (ProductLifecycleStatus)settings.DefaultLifecycleStatus,
            TrackingType = Enum.IsDefined(request.TrackingType) ? request.TrackingType : (InventoryTrackingType)settings.DefaultTrackingType,
            ValuationMethod = Enum.IsDefined(request.ValuationMethod) && (int)request.ValuationMethod > 0 ? request.ValuationMethod : (InventoryValuationMethod)settings.DefaultValuationMethod,
            ProcurementType = Enum.IsDefined(request.ProcurementType) && (int)request.ProcurementType > 0 ? request.ProcurementType : (ProcurementType)settings.DefaultProcurementType,
            ProductCategoryId = await ResolveDefaultId(request.ProductCategoryId, settings.DefaultProductCategoryId, unitOfWork.Repository<ProductCategory>(), ct),
            ProductGroupId = await ResolveDefaultId(request.ProductGroupId, settings.DefaultProductGroupId, unitOfWork.Repository<ProductGroup>(), ct),
            BrandId = request.BrandId ?? settings.DefaultBrandId,
            BaseUnitId = await ResolveDefaultId(request.BaseUnitId, settings.DefaultBaseUnitId, unitOfWork.Repository<UnitEntity>(), ct),
            PurchaseTaxGroupId = await ResolveDefaultId(request.PurchaseTaxGroupId, settings.DefaultPurchaseTaxGroupId, unitOfWork.TaxGroups, ct),
            SalesTaxGroupId = await ResolveDefaultId(request.SalesTaxGroupId, settings.DefaultSalesTaxGroupId, unitOfWork.TaxGroups, ct),
            ShelfLifeDays = request.ShelfLifeDays ?? settings.DefaultShelfLifeDays,
            IsPurchasable = request.IsPurchasable ?? settings.DefaultPurchasable,
            IsSellable = request.IsSellable ?? settings.DefaultSellable,
            IsInventoryTracked = request.IsInventoryTracked ?? settings.DefaultInventoryTracked
        };
        var code = await parameters.ResolveCodeAsync(request.Code, request.BranchId, productType, ct);
        if (string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(request.Name)) return ApiResponse<object>.Error(ProductMessages.Get("Required", culture), 400);
        if ((settings.RequireCountryOfOrigin && string.IsNullOrWhiteSpace(request.CountryOfOriginCode)) ||
            (settings.RequireCustomsTariffCode && string.IsNullOrWhiteSpace(request.CustomsTariffCode)) ||
            (settings.RequireNetWeight && request.NetWeight is not > 0) ||
            (settings.RequireGrossWeight && request.GrossWeight is not > 0) ||
            (settings.RequireShelfLife && effective.ShelfLifeDays is not > 0))
            return ApiResponse<object>.Error(ProductMessages.Get("Invalid", culture), 400);
        if (await unitOfWork.Repository<Product>().ExistsAsync(x => x.Code == code, ct)) return ApiResponse<object>.Error(ProductMessages.Get("DuplicateCode", culture), 409);
        if (!IsValid(effective)) return ApiResponse<object>.Error(ProductMessages.Get("Invalid", culture), 400);
        var product = Map(effective, code);
        await unitOfWork.ExecuteInTransactionAsync(async token =>
        {
            await unitOfWork.Repository<Product>().AddAsync(product, token);
            await unitOfWork.SaveChangesAsync(token);
        }, ct);
        return ApiResponse<object>.Ok(new { product.Id }, ProductMessages.Get("Created", culture));
    }

    public async Task<ApiResponse<SaveProductRequest>> GetByIdAsync(long id,CancellationToken ct)
    {
        var x=await unitOfWork.Repository<Product>().Query().Include(v=>v.Units).Include(v=>v.Barcodes).Include(v=>v.BranchSettings).Include(v=>v.Translations).FirstOrDefaultAsync(v=>v.Id==id,ct);
        if(x is null)return ApiResponse<SaveProductRequest>.Error("Product not found.",404);
        return ApiResponse<SaveProductRequest>.Ok(new(x.Code,x.Name,x.SearchName,x.Description,x.ProductType,x.LifecycleStatus,x.TrackingType,x.ValuationMethod,x.ProcurementType,x.ProductCategoryId,x.ProductGroupId,x.BrandId,x.BaseUnitId,x.PurchaseTaxGroupId,x.SalesTaxGroupId,x.CountryOfOriginCode,x.CustomsTariffCode,x.ManufacturerCode,x.NetWeight,x.GrossWeight,x.Volume,x.ShelfLifeDays,x.IsPurchasable,x.IsSellable,x.IsInventoryTracked,x.Units.Select(v=>new ProductUnitInput(v.UnitId,v.Numerator,v.Denominator,v.IsPurchaseUnit,v.IsSalesUnit,v.IsDefaultPurchaseUnit,v.IsDefaultSalesUnit)).ToList(),x.Barcodes.Select(v=>new ProductBarcodeInput(v.UnitId,v.Barcode,v.BarcodeType,v.IsPrimary)).ToList(),x.BranchSettings.Select(v=>new ProductBranchInput(v.BranchId,v.IsPurchasable,v.IsSellable,v.IsInventoryTracked,v.MinimumStock,v.MaximumStock,v.SafetyStock,v.ReorderPoint,v.ReorderQuantity,v.LeadTimeDays)).ToList(),x.Translations.Select(v=>new ProductTranslationInput(v.LanguageCode,v.Name,v.ShortDescription,v.Description)).ToList(),null));
    }

    public async Task<ApiResponse<object>> UpdateAsync(long id,SaveProductRequest request,string? culture,CancellationToken ct)
    {
        var existing=await unitOfWork.Repository<Product>().Query(true).Include(v=>v.Units).Include(v=>v.Barcodes).Include(v=>v.BranchSettings).Include(v=>v.Translations).FirstOrDefaultAsync(v=>v.Id==id,ct);
        if(existing is null)return ApiResponse<object>.Error("Product not found.",404);
        var code=request.Code.Trim().ToUpperInvariant();
        if(string.IsNullOrWhiteSpace(code)||string.IsNullOrWhiteSpace(request.Name)||!IsValid(request))return ApiResponse<object>.Error(ProductMessages.Get("Invalid",culture),400);
        if(await unitOfWork.Repository<Product>().ExistsAsync(v=>v.Id!=id&&v.Code==code,ct))return ApiResponse<object>.Error(ProductMessages.Get("DuplicateCode",culture),409);
        var replacement=Map(request,code);
        existing.Code=replacement.Code;existing.Name=replacement.Name;existing.SearchName=replacement.SearchName;existing.Description=replacement.Description;existing.ProductType=replacement.ProductType;existing.LifecycleStatus=replacement.LifecycleStatus;existing.TrackingType=replacement.TrackingType;existing.ValuationMethod=replacement.ValuationMethod;existing.ProcurementType=replacement.ProcurementType;existing.ProductCategoryId=replacement.ProductCategoryId;existing.ProductGroupId=replacement.ProductGroupId;existing.BrandId=replacement.BrandId;existing.BaseUnitId=replacement.BaseUnitId;existing.PurchaseTaxGroupId=replacement.PurchaseTaxGroupId;existing.SalesTaxGroupId=replacement.SalesTaxGroupId;existing.CountryOfOriginCode=replacement.CountryOfOriginCode;existing.CustomsTariffCode=replacement.CustomsTariffCode;existing.ManufacturerCode=replacement.ManufacturerCode;existing.NetWeight=replacement.NetWeight;existing.GrossWeight=replacement.GrossWeight;existing.Volume=replacement.Volume;existing.ShelfLifeDays=replacement.ShelfLifeDays;existing.IsPurchasable=replacement.IsPurchasable;existing.IsSellable=replacement.IsSellable;existing.IsInventoryTracked=replacement.IsInventoryTracked;existing.IsActive=replacement.IsActive;
        unitOfWork.Repository<ProductUnit>().RemoveRange(existing.Units);unitOfWork.Repository<ProductBarcode>().RemoveRange(existing.Barcodes);unitOfWork.Repository<ProductBranchSetting>().RemoveRange(existing.BranchSettings);unitOfWork.Repository<ProductTranslation>().RemoveRange(existing.Translations);
        existing.Units=replacement.Units;existing.Barcodes=replacement.Barcodes;existing.BranchSettings=replacement.BranchSettings;existing.Translations=replacement.Translations;
        await unitOfWork.SaveChangesAsync(ct);return ApiResponse<object>.Ok(new{existing.Id},"Product updated.");
    }

    public async Task<ApiResponse<object>> DeleteAsync(long id,CancellationToken ct)
    {
        var entity=await unitOfWork.Repository<Product>().GetByIdForUpdateAsync(id,ct);if(entity is null)return ApiResponse<object>.Error("Product not found.",404);
        try{unitOfWork.Repository<Product>().Remove(entity);await unitOfWork.SaveChangesAsync(ct);return ApiResponse<object>.Ok(new{entity.Id});}
        catch(DbUpdateException){return ApiResponse<object>.Error("Product has inventory or document history and cannot be deleted. Deactivate it instead.",409);}
    }

    public async Task<ApiResponse<PagedResult<ManagedProductDefinition>>> GetManagedDefinitionsAsync(string kind, ProductDefinitionListQuery request, string? culture, CancellationToken ct)
    {
        (IReadOnlyList<ManagedProductDefinition> Rows,int Total)? result=kind.Trim().ToLowerInvariant() switch{
            "categories"=>await ReadDefinitions(unitOfWork.Repository<ProductCategory>(),request,ct),"groups"=>await ReadDefinitions(unitOfWork.Repository<ProductGroup>(),request,ct),"brands"=>await ReadDefinitions(unitOfWork.Repository<Brand>(),request,ct),"unit-categories"=>await ReadDefinitions(unitOfWork.Repository<UnitCategory>(),request,ct),"units"=>await ReadDefinitions(unitOfWork.Repository<UnitEntity>(),request,ct),"package-types"=>await ReadDefinitions(unitOfWork.Repository<PackageType>(),request,ct),_=>null};
        return result is null?ApiResponse<PagedResult<ManagedProductDefinition>>.Error("Unknown definition type.",400):ApiResponse<PagedResult<ManagedProductDefinition>>.Ok(new(result.Value.Rows,request.NormalizedPageNumber,request.NormalizedPageSize,result.Value.Total),ProductMessages.Get("DefinitionsRetrieved",culture));
    }

    public Task<ApiResponse<object>> CreateDefinitionAsync(string kind,SaveProductDefinitionRequest request,string? culture,CancellationToken ct)=>kind.Trim().ToLowerInvariant() switch{
        "categories"=>CreateDefinition(unitOfWork.Repository<ProductCategory>(),()=>new ProductCategory(),request,culture,ct),"groups"=>CreateDefinition(unitOfWork.Repository<ProductGroup>(),()=>new ProductGroup(),request,culture,ct),"brands"=>CreateDefinition(unitOfWork.Repository<Brand>(),()=>new Brand(),request,culture,ct),"unit-categories"=>CreateDefinition(unitOfWork.Repository<UnitCategory>(),()=>new UnitCategory(),request,culture,ct),"units"=>CreateDefinition(unitOfWork.Repository<UnitEntity>(),()=>new UnitEntity(),request,culture,ct),"package-types"=>CreateDefinition(unitOfWork.Repository<PackageType>(),()=>new PackageType(),request,culture,ct),_=>Task.FromResult(ApiResponse<object>.Error("Unknown definition type.",400))};
    public Task<ApiResponse<object>> UpdateDefinitionAsync(string kind,long id,SaveProductDefinitionRequest request,string? culture,CancellationToken ct)=>kind.Trim().ToLowerInvariant() switch{
        "categories"=>UpdateDefinition(unitOfWork.Repository<ProductCategory>(),id,request,culture,ct),"groups"=>UpdateDefinition(unitOfWork.Repository<ProductGroup>(),id,request,culture,ct),"brands"=>UpdateDefinition(unitOfWork.Repository<Brand>(),id,request,culture,ct),"unit-categories"=>UpdateDefinition(unitOfWork.Repository<UnitCategory>(),id,request,culture,ct),"units"=>UpdateDefinition(unitOfWork.Repository<UnitEntity>(),id,request,culture,ct),"package-types"=>UpdateDefinition(unitOfWork.Repository<PackageType>(),id,request,culture,ct),_=>Task.FromResult(ApiResponse<object>.Error("Unknown definition type.",400))};
    public Task<ApiResponse<object>> DeleteDefinitionAsync(string kind,long id,string? culture,CancellationToken ct)=>kind.Trim().ToLowerInvariant() switch{
        "categories"=>DeactivateDefinition(unitOfWork.Repository<ProductCategory>(),id,ct),"groups"=>DeactivateDefinition(unitOfWork.Repository<ProductGroup>(),id,ct),"brands"=>DeactivateDefinition(unitOfWork.Repository<Brand>(),id,ct),"unit-categories"=>DeactivateDefinition(unitOfWork.Repository<UnitCategory>(),id,ct),"units"=>DeactivateDefinition(unitOfWork.Repository<UnitEntity>(),id,ct),"package-types"=>DeactivateDefinition(unitOfWork.Repository<PackageType>(),id,ct),_=>Task.FromResult(ApiResponse<object>.Error("Unknown definition type.",400))};

    private static async Task<(IReadOnlyList<ManagedProductDefinition> Rows,int Total)> ReadDefinitions<T>(IGenericRepository<T> repository,ProductDefinitionListQuery request,CancellationToken ct)where T:DefinitionEntity{var query=repository.Query();if(request.IsActive.HasValue)query=query.Where(x=>x.IsActive==request.IsActive);var search=request.Search?.Trim();if(!string.IsNullOrWhiteSpace(search))query=query.Where(x=>x.Code.Contains(search)||x.Name.Contains(search)||(x.Description!=null&&x.Description.Contains(search)));var desc=request.IsDescending;query=request.SortBy.Trim().ToLowerInvariant()switch{"name"=>desc?query.OrderByDescending(x=>x.Name):query.OrderBy(x=>x.Name),"displayorder"=>desc?query.OrderByDescending(x=>x.DisplayOrder):query.OrderBy(x=>x.DisplayOrder),_=>desc?query.OrderByDescending(x=>x.Code):query.OrderBy(x=>x.Code)};query=query.ApplyPagedFilters(request);var total=await query.CountAsync(ct);var entities=await query.Skip((request.NormalizedPageNumber-1)*request.NormalizedPageSize).Take(request.NormalizedPageSize).ToListAsync(ct);return(entities.Select(MapDefinition).ToList(),total);}
    private static ManagedProductDefinition MapDefinition(DefinitionEntity x)=>new(x.Id,x.Code,x.Name,x.Description,x.IsActive,x.IsDefault,x.DisplayOrder,x is ProductCategory c?c.ParentId:null,x is Brand b?b.Website:null,x is UnitEntity u?u.UnitCategoryId:null,x is UnitEntity u2?u2.Symbol:null,x is UnitEntity u3?u3.DecimalPlaces:null,x is UnitEntity u4?u4.ConversionFactor:null,x is UnitEntity u5?u5.IsBaseUnit:null,x is UnitEntity u6?(int)u6.RoundingMethod:null,x is PackageType p?p.Length:null,x is PackageType p2?p2.Width:null,x is PackageType p3?p3.Height:null,x is PackageType p4?p4.EmptyWeight:null,x is PackageType p5?p5.DimensionUnitId:null,x is PackageType p6?p6.WeightUnitId:null);
    private async Task<ApiResponse<object>> CreateDefinition<T>(IGenericRepository<T> repository,Func<T> factory,SaveProductDefinitionRequest request,string? culture,CancellationToken ct)where T:DefinitionEntity{var code=request.Code.Trim().ToUpperInvariant();if(string.IsNullOrWhiteSpace(code)||string.IsNullOrWhiteSpace(request.Name))return ApiResponse<object>.Error(ProductMessages.Get("Required",culture),400);if(await repository.ExistsAsync(x=>x.Code==code,ct))return ApiResponse<object>.Error(ProductMessages.Get("DuplicateCode",culture),409);var entity=factory();ApplyDefinition(entity,request,code);await unitOfWork.ExecuteInTransactionAsync(async token=>{if(entity.IsDefault)await ClearDefaults(repository,null,token);await repository.AddAsync(entity,token);await unitOfWork.SaveChangesAsync(token);},ct);return ApiResponse<object>.Ok(new{entity.Id},ProductMessages.Get("Created",culture));}
    private async Task<ApiResponse<object>> UpdateDefinition<T>(IGenericRepository<T> repository,long id,SaveProductDefinitionRequest request,string? culture,CancellationToken ct)where T:DefinitionEntity{var entity=await repository.GetByIdForUpdateAsync(id,ct);if(entity is null)return ApiResponse<object>.Error("Definition not found.",404);var code=request.Code.Trim().ToUpperInvariant();if(await repository.ExistsAsync(x=>x.Id!=id&&x.Code==code,ct))return ApiResponse<object>.Error(ProductMessages.Get("DuplicateCode",culture),409);await unitOfWork.ExecuteInTransactionAsync(async token=>{if(request.IsDefault)await ClearDefaults(repository,id,token);ApplyDefinition(entity,request,code);repository.Update(entity);await unitOfWork.SaveChangesAsync(token);},ct);return ApiResponse<object>.Ok(new{entity.Id},ProductMessages.Get("Created",culture));}
    private async Task<ApiResponse<object>> DeactivateDefinition<T>(IGenericRepository<T> repository,long id,CancellationToken ct)where T:DefinitionEntity{var entity=await repository.GetByIdForUpdateAsync(id,ct);if(entity is null)return ApiResponse<object>.Error("Definition not found.",404);if(entity.IsDefault)return ApiResponse<object>.Error("Default definition cannot be deleted.",409);try{repository.Remove(entity);await unitOfWork.SaveChangesAsync(ct);return ApiResponse<object>.Ok(new{entity.Id});}catch(DbUpdateException){return ApiResponse<object>.Error("This definition is in use and cannot be deleted. Replace its references first.",409);}}
    private static void ApplyDefinition(DefinitionEntity entity,SaveProductDefinitionRequest r,string code){entity.Code=code;entity.Name=r.Name.Trim();entity.Description=r.Description?.Trim();entity.IsActive=r.IsActive;entity.IsDefault=r.IsDefault;entity.DisplayOrder=r.DisplayOrder;if(entity is ProductCategory c)c.ParentId=r.ParentId;if(entity is Brand b)b.Website=r.Website?.Trim();if(entity is UnitEntity u){u.UnitCategoryId=r.UnitCategoryId??throw new ArgumentException("Unit category is required.");u.Symbol=r.Symbol?.Trim()??code;u.DecimalPlaces=Math.Clamp(r.DecimalPlaces??0,0,6);u.ConversionFactor=r.ConversionFactor??1;u.IsBaseUnit=r.IsBaseUnit??false;u.RoundingMethod=(RoundingMethod)(r.RoundingMethod??0);}if(entity is PackageType p){p.Length=r.Length;p.Width=r.Width;p.Height=r.Height;p.EmptyWeight=r.EmptyWeight;p.DimensionUnitId=r.DimensionUnitId;p.WeightUnitId=r.WeightUnitId;}}
    private static async Task ClearDefaults<T>(IGenericRepository<T> repository,long? exceptId,CancellationToken ct)where T:DefinitionEntity{var rows=await repository.Query(tracking:true).Where(x=>x.IsDefault&&(!exceptId.HasValue||x.Id!=exceptId)).ToListAsync(ct);foreach(var row in rows)row.IsDefault=false;}

    private static Product Map(SaveProductRequest r, string code)
    {
        var product = new Product { Code = code, Name = r.Name.Trim(), SearchName = r.SearchName?.Trim(), Description = r.Description?.Trim(),
            ProductType = r.ProductType, LifecycleStatus = r.LifecycleStatus, TrackingType = r.TrackingType, ValuationMethod = r.ValuationMethod,
            ProcurementType = r.ProcurementType, ProductCategoryId = r.ProductCategoryId, ProductGroupId = r.ProductGroupId, BrandId = r.BrandId,
            BaseUnitId = r.BaseUnitId, PurchaseTaxGroupId = r.PurchaseTaxGroupId, SalesTaxGroupId = r.SalesTaxGroupId,
            CountryOfOriginCode = r.CountryOfOriginCode?.Trim().ToUpperInvariant(), CustomsTariffCode = r.CustomsTariffCode?.Trim(), ManufacturerCode = r.ManufacturerCode?.Trim(),
            NetWeight = r.NetWeight, GrossWeight = r.GrossWeight, Volume = r.Volume, ShelfLifeDays = r.ShelfLifeDays,
            IsPurchasable = r.IsPurchasable ?? false, IsSellable = r.IsSellable ?? false, IsInventoryTracked = r.ProductType != ProductType.Service && (r.IsInventoryTracked ?? false),
            IsActive = r.LifecycleStatus is ProductLifecycleStatus.Active or ProductLifecycleStatus.Draft };
        foreach (var x in r.Units ?? []) product.Units.Add(new ProductUnit { UnitId = x.UnitId, Numerator = x.Numerator, Denominator = x.Denominator, IsPurchaseUnit = x.IsPurchaseUnit, IsSalesUnit = x.IsSalesUnit, IsDefaultPurchaseUnit = x.IsDefaultPurchaseUnit, IsDefaultSalesUnit = x.IsDefaultSalesUnit });
        if (product.Units.All(x => x.UnitId != r.BaseUnitId)) product.Units.Add(new ProductUnit { UnitId = r.BaseUnitId, Numerator = 1, Denominator = 1, IsPurchaseUnit = true, IsSalesUnit = true, IsDefaultPurchaseUnit = true, IsDefaultSalesUnit = true });
        foreach (var x in r.Barcodes ?? []) product.Barcodes.Add(new ProductBarcode { UnitId = x.UnitId, Barcode = x.Barcode.Trim(), BarcodeType = x.BarcodeType, IsPrimary = x.IsPrimary });
        foreach (var x in r.BranchSettings ?? []) product.BranchSettings.Add(new ProductBranchSetting { BranchId = x.BranchId, IsPurchasable = x.IsPurchasable, IsSellable = x.IsSellable, IsInventoryTracked = x.IsInventoryTracked, MinimumStock = x.MinimumStock, MaximumStock = x.MaximumStock, SafetyStock = x.SafetyStock, ReorderPoint = x.ReorderPoint, ReorderQuantity = x.ReorderQuantity, LeadTimeDays = x.LeadTimeDays });
        foreach (var x in r.Translations ?? []) product.Translations.Add(new ProductTranslation { LanguageCode = x.LanguageCode.Trim().ToLowerInvariant(), Name = x.Name.Trim(), ShortDescription = x.ShortDescription?.Trim(), Description = x.Description?.Trim() });
        return product;
    }

    private static bool IsValid(SaveProductRequest r) => r.ProductCategoryId > 0 && r.ProductGroupId > 0 && r.BaseUnitId > 0 && r.PurchaseTaxGroupId > 0 && r.SalesTaxGroupId > 0 &&
        (r.ShelfLifeDays is null or >= 0) && (r.Units?.All(x => x.UnitId > 0 && x.Numerator > 0 && x.Denominator > 0) ?? true) &&
        (r.Barcodes?.All(x => !string.IsNullOrWhiteSpace(x.Barcode)) ?? true) && (r.Barcodes?.Count(x => x.IsPrimary) ?? 0) <= 1 &&
        (r.BranchSettings?.Select(x => x.BranchId).Distinct().Count() ?? 0) == (r.BranchSettings?.Count ?? 0) &&
        (r.Translations?.Select(x => x.LanguageCode.Trim().ToLowerInvariant()).Distinct().Count() ?? 0) == (r.Translations?.Count ?? 0);

    private static async Task<long> ResolveDefaultId<T>(long requested,long? configured,IGenericRepository<T> repository,CancellationToken ct) where T:DefinitionEntity
    {
        if(requested>0)return requested;
        if(configured.HasValue)return configured.Value;
        return await repository.Query().Where(x=>x.IsActive&&x.IsDefault).Select(x=>x.Id).FirstOrDefaultAsync(ct);
    }

    private static IQueryable<Product> ApplySort(IQueryable<Product> query, string? sortBy, bool descending) => (sortBy ?? "code").Trim().ToLowerInvariant() switch
    {
        "name" => descending ? query.OrderByDescending(x => x.Name) : query.OrderBy(x => x.Name),
        "category" => descending ? query.OrderByDescending(x => x.ProductCategory.Name) : query.OrderBy(x => x.ProductCategory.Name),
        "group" => descending ? query.OrderByDescending(x => x.ProductGroup.Name) : query.OrderBy(x => x.ProductGroup.Name),
        "createdat" => descending ? query.OrderByDescending(x => x.CreatedAt) : query.OrderBy(x => x.CreatedAt),
        _ => descending ? query.OrderByDescending(x => x.Code) : query.OrderBy(x => x.Code)
    };
}


