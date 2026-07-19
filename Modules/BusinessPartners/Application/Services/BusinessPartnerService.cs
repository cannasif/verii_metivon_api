using Microsoft.EntityFrameworkCore;
using verii_metivon_api.Core.Auth;
using verii_metivon_api.Core.Domain;
using verii_metivon_api.Core.Paging;
using verii_metivon_api.Core.Repositories;
using verii_metivon_api.Core.UnitOfWork;
using verii_metivon_api.Modules.BusinessPartners.Localization;
using verii_metivon_api.Modules.Parameters.Application;

namespace verii_metivon_api.Modules.BusinessPartners.Application.Services;

public sealed class BusinessPartnerService(IUnitOfWork unitOfWork, IParameterService parameters) : IBusinessPartnerService
{
    private static readonly IReadOnlyDictionary<string, string> ListFilterAliases =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["partnerType"] = "BusinessPartnerType.Name",
            ["customerGroup"] = "CustomerGroup.Name",
            ["paymentTerm"] = "PaymentTerm.Name",
            ["currency"] = "Currency.IsoCode",
            ["taxGroup"] = "TaxGroup.Name"
        };

    public async Task<ApiResponse<PagedResult<BusinessPartnerItem>>> GetAllAsync(BusinessPartnerListQuery request, string? culture, CancellationToken ct)
    {
        var pageNumber = request.NormalizedPageNumber;
        var pageSize = request.NormalizedPageSize;
        var query = unitOfWork.BusinessPartners.Query();
        if (request.IsActive.HasValue) query = query.Where(x => x.IsActive == request.IsActive.Value);
        var search = request.Search?.Trim();
        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(x => x.Code.Contains(search) || x.Name.Contains(search) ||
                (x.LegalName != null && x.LegalName.Contains(search)) || (x.TaxNumber != null && x.TaxNumber.Contains(search)) ||
                (x.NationalIdentityNumber != null && x.NationalIdentityNumber.Contains(search)) ||
                (x.Email != null && x.Email.Contains(search)) || (x.Phone != null && x.Phone.Contains(search)));

        query = query.ApplyPagedFilters(request, ListFilterAliases);
        var descending = request.IsDescending;
        query = request.SortBy.Trim().ToLowerInvariant() switch
        {
            "name" => descending ? query.OrderByDescending(x => x.Name) : query.OrderBy(x => x.Name),
            "creditlimit" => descending ? query.OrderByDescending(x => x.CreditLimit) : query.OrderBy(x => x.CreditLimit),
            "createdat" => descending ? query.OrderByDescending(x => x.CreatedAt) : query.OrderBy(x => x.CreatedAt),
            _ => descending ? query.OrderByDescending(x => x.Code) : query.OrderBy(x => x.Code)
        };
        var totalCount = await query.CountAsync(ct);
        var rows = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize)
            .Select(x => new BusinessPartnerItem(x.Id, x.Code, x.Name, x.BusinessPartnerType.Name,
                x.CustomerGroup != null ? x.CustomerGroup.Name : null, x.PaymentTerm.Name, x.Currency.IsoCode,
                x.TaxGroup.Name, x.CreditLimit, x.HasUnlimitedCredit, x.IsActive)).ToListAsync(ct);
        return ApiResponse<PagedResult<BusinessPartnerItem>>.Ok(
            new PagedResult<BusinessPartnerItem>(rows, pageNumber, pageSize, totalCount),
            BusinessPartnerMessages.Get("Retrieved", culture));
    }

    public async Task<ApiResponse<BusinessPartnerDefinitions>> GetDefinitionsAsync(string? culture, CancellationToken ct)
    {
        static async Task<IReadOnlyList<DefinitionItem>> Read<T>(IGenericRepository<T> repository, CancellationToken token)
            where T : DefinitionEntity => await repository.Query().Where(x => x.IsActive).OrderBy(x => x.DisplayOrder)
                .ThenBy(x => x.Name).Select(x => new DefinitionItem(x.Id, x.Code, x.Name, x.IsDefault)).ToListAsync(token);
        var data = new BusinessPartnerDefinitions(await Read(unitOfWork.BusinessPartnerTypes, ct), await Read(unitOfWork.CustomerGroups, ct),
            await Read(unitOfWork.PaymentTerms, ct), await Read(unitOfWork.Currencies, ct), await Read(unitOfWork.TaxGroups, ct));
        return ApiResponse<BusinessPartnerDefinitions>.Ok(data, BusinessPartnerMessages.Get("DefinitionsRetrieved", culture));
    }

    public async Task<ApiResponse<BusinessPartnerDetail>> GetByIdAsync(long id,string? culture,CancellationToken ct)
    {
        var item=await unitOfWork.BusinessPartners.Query().Where(x=>x.Id==id).Select(x=>new BusinessPartnerDetail(x.Id,x.Code,x.Name,x.LegalName,x.BranchId,x.BusinessPartnerTypeId,x.CustomerGroupId,x.PaymentTermId,x.CurrencyId,x.TaxGroupId,x.TaxOffice,x.TaxNumber,x.NationalIdentityNumber,x.Email,x.Phone,x.MobilePhone,x.Website,x.CreditLimit,x.HasUnlimitedCredit,x.IsActive,x.Notes)).FirstOrDefaultAsync(ct);
        return item is null?ApiResponse<BusinessPartnerDetail>.Error("Business partner not found.",404):ApiResponse<BusinessPartnerDetail>.Ok(item);
    }

    public async Task<ApiResponse<object>> UpdateAsync(long id,CreateBusinessPartnerRequest request,string? culture,CancellationToken ct)
    {
        var entity=await unitOfWork.BusinessPartners.GetByIdForUpdateAsync(id,ct);
        if(entity is null)return ApiResponse<object>.Error("Business partner not found.",404);
        if(string.IsNullOrWhiteSpace(request.Code)||string.IsNullOrWhiteSpace(request.Name)||request.BusinessPartnerTypeId is not >0||request.PaymentTermId is not >0||request.CurrencyId is not >0||request.TaxGroupId is not >0)return ApiResponse<object>.Error(BusinessPartnerMessages.Get("Required",culture),400);
        var code=request.Code.Trim().ToUpperInvariant();var tax=request.TaxNumber?.Trim();var national=request.NationalIdentityNumber?.Trim();var email=request.Email?.Trim().ToLowerInvariant();
        if(await unitOfWork.BusinessPartners.ExistsAsync(x=>x.Id!=id&&x.Code==code,ct))return ApiResponse<object>.Error(BusinessPartnerMessages.Get("DuplicateCode",culture),409);
        if(!string.IsNullOrWhiteSpace(tax)&&await unitOfWork.BusinessPartners.ExistsAsync(x=>x.Id!=id&&x.TaxNumber==tax,ct))return ApiResponse<object>.Error(BusinessPartnerMessages.Get("DuplicateTaxNumber",culture),409);
        if(!string.IsNullOrWhiteSpace(national)&&await unitOfWork.BusinessPartners.ExistsAsync(x=>x.Id!=id&&x.NationalIdentityNumber==national,ct))return ApiResponse<object>.Error(BusinessPartnerMessages.Get("DuplicateNationalId",culture),409);
        entity.Code=code;entity.Name=request.Name.Trim();entity.LegalName=request.LegalName?.Trim();entity.BranchId=request.BranchId;entity.BusinessPartnerTypeId=request.BusinessPartnerTypeId.Value;entity.CustomerGroupId=request.CustomerGroupId;entity.PaymentTermId=request.PaymentTermId.Value;entity.CurrencyId=request.CurrencyId.Value;entity.TaxGroupId=request.TaxGroupId.Value;entity.TaxOffice=request.TaxOffice?.Trim();entity.TaxNumber=tax;entity.NationalIdentityNumber=national;entity.Email=email;entity.Phone=request.Phone?.Trim();entity.MobilePhone=request.MobilePhone?.Trim();entity.Website=request.Website?.Trim();entity.CreditLimit=request.CreditLimit??0;entity.HasUnlimitedCredit=request.HasUnlimitedCredit??false;entity.Notes=request.Notes?.Trim();
        unitOfWork.BusinessPartners.Update(entity);await unitOfWork.SaveChangesAsync(ct);return ApiResponse<object>.Ok(new{entity.Id,entity.Code});
    }

    public async Task<ApiResponse<object>> CreateAsync(CreateBusinessPartnerRequest request, string? culture, CancellationToken ct)
    {
        var settings = await parameters.GetBusinessPartnerParametersAsync(request.BranchId, ct);
        var partnerTypeId = request.BusinessPartnerTypeId is > 0 ? request.BusinessPartnerTypeId : settings.DefaultBusinessPartnerTypeId;
        var paymentTermId = request.PaymentTermId is > 0 ? request.PaymentTermId : settings.DefaultPaymentTermId;
        var currencyId = request.CurrencyId is > 0 ? request.CurrencyId : settings.DefaultCurrencyId;
        var taxGroupId = request.TaxGroupId is > 0 ? request.TaxGroupId : settings.DefaultTaxGroupId;
        var customerGroupId = request.CustomerGroupId is > 0 ? request.CustomerGroupId : settings.DefaultCustomerGroupId;
        if (string.IsNullOrWhiteSpace(request.Name) || !partnerTypeId.HasValue || !paymentTermId.HasValue || !currencyId.HasValue || !taxGroupId.HasValue)
            return ApiResponse<object>.Error(BusinessPartnerMessages.Get("Required", culture), 400);
        if (settings.RequireTaxNumber && string.IsNullOrWhiteSpace(request.TaxNumber))
            return ApiResponse<object>.Error(BusinessPartnerMessages.Get("TaxNumberRequired", culture), 400);
        if (settings.RequireTaxOffice && string.IsNullOrWhiteSpace(request.TaxOffice))
            return ApiResponse<object>.Error(BusinessPartnerMessages.Get("TaxOfficeRequired", culture), 400);
        if (settings.RequireEmail && string.IsNullOrWhiteSpace(request.Email))
            return ApiResponse<object>.Error(BusinessPartnerMessages.Get("EmailRequired", culture), 400);
        if (settings.RequirePhone && string.IsNullOrWhiteSpace(request.Phone) && string.IsNullOrWhiteSpace(request.MobilePhone))
            return ApiResponse<object>.Error(BusinessPartnerMessages.Get("PhoneRequired", culture), 400);
        if (settings.RequireNationalIdentityNumber && string.IsNullOrWhiteSpace(request.NationalIdentityNumber))
            return ApiResponse<object>.Error(BusinessPartnerMessages.Get("NationalIdRequired", culture), 400);
        if (!string.IsNullOrWhiteSpace(request.NationalIdentityNumber) && request.NationalIdentityNumber.Trim().Length != 11)
            return ApiResponse<object>.Error(BusinessPartnerMessages.Get("InvalidTckn", culture), 400);
        try
        {
        return await unitOfWork.ExecuteInTransactionAsync(async token =>
        {
        var code = await parameters.ResolveBusinessPartnerCodeAsync(request.Code, request.BranchId, partnerTypeId.Value, token);
        if (string.IsNullOrWhiteSpace(code)) return ApiResponse<object>.Error(BusinessPartnerMessages.Get("Required", culture), 400);
        if (await unitOfWork.BusinessPartners.ExistsAsync(x => x.Code == code, token)) return ApiResponse<object>.Error(BusinessPartnerMessages.Get("DuplicateCode", culture), 409);
        var taxNumber = request.TaxNumber?.Trim();
        var nationalId = request.NationalIdentityNumber?.Trim();
        var email = request.Email?.Trim().ToLowerInvariant();
        if (settings.PreventDuplicateTaxNumber && !string.IsNullOrWhiteSpace(taxNumber) &&
            await unitOfWork.BusinessPartners.ExistsAsync(x => x.TaxNumber == taxNumber, token))
            return ApiResponse<object>.Error(BusinessPartnerMessages.Get("DuplicateTaxNumber", culture), 409);
        if (settings.PreventDuplicateNationalIdentityNumber && !string.IsNullOrWhiteSpace(nationalId) &&
            await unitOfWork.BusinessPartners.ExistsAsync(x => x.NationalIdentityNumber == nationalId, token))
            return ApiResponse<object>.Error(BusinessPartnerMessages.Get("DuplicateNationalId", culture), 409);
        if (settings.PreventDuplicateEmail && !string.IsNullOrWhiteSpace(email) &&
            await unitOfWork.BusinessPartners.ExistsAsync(x => x.Email != null && x.Email.ToLower() == email, token))
            return ApiResponse<object>.Error(BusinessPartnerMessages.Get("DuplicateEmail", culture), 409);
        var partner = new BusinessPartner { Code = code, Name = request.Name.Trim(), LegalName = request.LegalName?.Trim(), BranchId = request.BranchId,
            BusinessPartnerTypeId = partnerTypeId.Value, CustomerGroupId = customerGroupId, PaymentTermId = paymentTermId.Value,
            CurrencyId = currencyId.Value, TaxGroupId = taxGroupId.Value, TaxOffice = request.TaxOffice?.Trim(), TaxNumber = taxNumber,
            NationalIdentityNumber = nationalId, Email = email, Phone = request.Phone?.Trim(),
            MobilePhone = request.MobilePhone?.Trim(), Website = request.Website?.Trim(), CreditLimit = request.CreditLimit ?? settings.DefaultCreditLimit,
            HasUnlimitedCredit = request.HasUnlimitedCredit ?? settings.DefaultUnlimitedCredit, Notes = request.Notes?.Trim(), IsActive = settings.CreateActiveByDefault };
        await unitOfWork.BusinessPartners.AddAsync(partner, token); await unitOfWork.SaveChangesAsync(token);
        return ApiResponse<object>.Ok(new { partner.Id, partner.Code }, BusinessPartnerMessages.Get("Created", culture));
        }, ct);
        }
        catch (InvalidOperationException exception) { return ApiResponse<object>.Error(exception.Message, 409); }
    }

    public async Task<ApiResponse<PagedResult<ManagedDefinitionItem>>> GetManagedDefinitionsAsync(string kind, DefinitionListQuery request, string? culture, CancellationToken ct)
    {
        var pageNumber = request.NormalizedPageNumber;
        var pageSize = request.NormalizedPageSize;
        (IReadOnlyList<ManagedDefinitionItem> Rows, int TotalCount)? result = kind.Trim().ToLowerInvariant() switch
        {
            "partner-types" => await ReadManaged(unitOfWork.BusinessPartnerTypes, request, ct),
            "customer-groups" => await ReadManaged(unitOfWork.CustomerGroups, request, ct),
            "payment-terms" => await ReadManaged(unitOfWork.PaymentTerms, request, ct),
            "currencies" => await ReadManaged(unitOfWork.Currencies, request, ct),
            "tax-groups" => await ReadManaged(unitOfWork.TaxGroups, request, ct),
            _ => null
        };
        return result is null
            ? ApiResponse<PagedResult<ManagedDefinitionItem>>.Error("Unknown definition type.", 400)
            : ApiResponse<PagedResult<ManagedDefinitionItem>>.Ok(new PagedResult<ManagedDefinitionItem>(result.Value.Rows, pageNumber, pageSize, result.Value.TotalCount), BusinessPartnerMessages.Get("DefinitionsRetrieved", culture));
    }

    public Task<ApiResponse<object>> CreateDefinitionAsync(string kind, SaveDefinitionRequest request, string? culture, CancellationToken ct) => kind.Trim().ToLowerInvariant() switch
    {
        "partner-types" => CreateDefinition(unitOfWork.BusinessPartnerTypes, kind, request, culture, ct),
        "customer-groups" => CreateDefinition(unitOfWork.CustomerGroups, kind, request, culture, ct),
        "payment-terms" => CreateDefinition(unitOfWork.PaymentTerms, kind, request, culture, ct),
        "currencies" => CreateDefinition(unitOfWork.Currencies, kind, request, culture, ct),
        "tax-groups" => CreateDefinition(unitOfWork.TaxGroups, kind, request, culture, ct),
        _ => Task.FromResult(ApiResponse<object>.Error("Unknown definition type.", 400))
    };

    private async Task<ApiResponse<object>> CreateDefinition<T>(IGenericRepository<T> repository, string kind, SaveDefinitionRequest request, string? culture, CancellationToken ct) where T : DefinitionEntity
        {
            var code = request.Code.Trim().ToUpperInvariant();
            if (string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(request.Name)) return ApiResponse<object>.Error(BusinessPartnerMessages.Get("Required", culture), 400);
            if (await repository.ExistsAsync(x => x.Code == code, ct)) return ApiResponse<object>.Error(BusinessPartnerMessages.Get("DuplicateCode", culture), 409);
            var entity = (T)CreateDefinitionEntity(kind, request, code);
            await unitOfWork.ExecuteInTransactionAsync(async token =>
            {
                if (entity.IsDefault) await ClearDefaults(repository, null, token);
                await repository.AddAsync(entity, token);
                await unitOfWork.SaveChangesAsync(token);
            }, ct);
            return ApiResponse<object>.Ok(new { entity.Id }, BusinessPartnerMessages.Get("Created", culture));
        }

    public Task<ApiResponse<object>> UpdateDefinitionAsync(string kind, long id, SaveDefinitionRequest request, string? culture, CancellationToken ct) => kind.Trim().ToLowerInvariant() switch
    {
        "partner-types" => UpdateDefinition(unitOfWork.BusinessPartnerTypes, id, request, culture, ct),
        "customer-groups" => UpdateDefinition(unitOfWork.CustomerGroups, id, request, culture, ct),
        "payment-terms" => UpdateDefinition(unitOfWork.PaymentTerms, id, request, culture, ct),
        "currencies" => UpdateDefinition(unitOfWork.Currencies, id, request, culture, ct),
        "tax-groups" => UpdateDefinition(unitOfWork.TaxGroups, id, request, culture, ct),
        _ => Task.FromResult(ApiResponse<object>.Error("Unknown definition type.", 400))
    };

    private async Task<ApiResponse<object>> UpdateDefinition<T>(IGenericRepository<T> repository, long id, SaveDefinitionRequest request, string? culture, CancellationToken ct) where T : DefinitionEntity
        {
            var entity = await repository.GetByIdForUpdateAsync(id, ct);
            if (entity is null) return ApiResponse<object>.Error("Definition not found.", 404);
            var code = request.Code.Trim().ToUpperInvariant();
            if (string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(request.Name)) return ApiResponse<object>.Error(BusinessPartnerMessages.Get("Required", culture), 400);
            if (await repository.ExistsAsync(x => x.Id != id && x.Code == code, ct)) return ApiResponse<object>.Error(BusinessPartnerMessages.Get("DuplicateCode", culture), 409);
            await unitOfWork.ExecuteInTransactionAsync(async token =>
            {
                if (request.IsDefault) await ClearDefaults(repository, id, token);
                ApplyDefinition(entity, request, code);
                repository.Update(entity);
                await unitOfWork.SaveChangesAsync(token);
            }, ct);
            return ApiResponse<object>.Ok(new { entity.Id }, BusinessPartnerMessages.Get("Created", culture));
        }

    public Task<ApiResponse<object>> DeleteDefinitionAsync(string kind, long id, string? culture, CancellationToken ct) => kind.Trim().ToLowerInvariant() switch
    {
        "partner-types" => DeleteDefinition(unitOfWork.BusinessPartnerTypes, id, culture, ct),
        "customer-groups" => DeleteDefinition(unitOfWork.CustomerGroups, id, culture, ct),
        "payment-terms" => DeleteDefinition(unitOfWork.PaymentTerms, id, culture, ct),
        "currencies" => DeleteDefinition(unitOfWork.Currencies, id, culture, ct),
        "tax-groups" => DeleteDefinition(unitOfWork.TaxGroups, id, culture, ct),
        _ => Task.FromResult(ApiResponse<object>.Error("Unknown definition type.", 400))
    };

    private async Task<ApiResponse<object>> DeleteDefinition<T>(IGenericRepository<T> repository, long id, string? culture, CancellationToken ct) where T : DefinitionEntity
        {
            var entity = await repository.GetByIdForUpdateAsync(id, ct);
            if (entity is null) return ApiResponse<object>.Error("Definition not found.", 404);
            if (entity.IsDefault) return ApiResponse<object>.Error("Default definition cannot be deleted.", 409);
            entity.IsActive = false;
            repository.Update(entity);
            await unitOfWork.SaveChangesAsync(ct);
            return ApiResponse<object>.Ok(new { entity.Id }, BusinessPartnerMessages.Get("Created", culture));
        }

    private static async Task<(IReadOnlyList<ManagedDefinitionItem> Rows, int TotalCount)> ReadManaged<T>(IGenericRepository<T> repository, DefinitionListQuery request, CancellationToken ct) where T : DefinitionEntity
    {
        var query = repository.Query();
        if (request.IsActive.HasValue) query = query.Where(x => x.IsActive == request.IsActive.Value);
        var search = request.Search?.Trim();
        if (!string.IsNullOrWhiteSpace(search)) query = query.Where(x => x.Code.Contains(search) || x.Name.Contains(search) || (x.Description != null && x.Description.Contains(search)));
        query = query.ApplyPagedFilters(request);
        var descending = request.IsDescending;
        query = request.SortBy.Trim().ToLowerInvariant() switch
        {
            "name" => descending ? query.OrderByDescending(x => x.Name) : query.OrderBy(x => x.Name),
            "displayorder" => descending ? query.OrderByDescending(x => x.DisplayOrder) : query.OrderBy(x => x.DisplayOrder),
            _ => descending ? query.OrderByDescending(x => x.Code) : query.OrderBy(x => x.Code)
        };
        var totalCount = await query.CountAsync(ct);
        var entities = await query.Skip((request.NormalizedPageNumber - 1) * request.NormalizedPageSize).Take(request.NormalizedPageSize).ToListAsync(ct);
        var rows = entities.Select(x => new ManagedDefinitionItem(
            x.Id, x.Code, x.Name, x.Description, x.IsActive, x.IsDefault, x.DisplayOrder,
            x is PaymentTerm payment ? payment.DueDays : null,
            x is PaymentTerm payment2 ? payment2.DiscountDays : null,
            x is PaymentTerm payment3 ? payment3.DiscountRate : null,
            x is Currency currency ? currency.IsoCode : null,
            x is Currency currency2 ? currency2.Symbol : null,
            x is Currency currency3 ? currency3.DecimalPlaces : null,
            x is TaxGroup tax ? tax.IsTaxExempt : null)).ToList();
        return (rows, totalCount);
    }

    private static DefinitionEntity CreateDefinitionEntity(string kind, SaveDefinitionRequest request, string code)
    {
        DefinitionEntity entity = kind.Trim().ToLowerInvariant() switch
        {
            "partner-types" => new BusinessPartnerType(), "customer-groups" => new CustomerGroup(),
            "payment-terms" => new PaymentTerm(), "currencies" => new Currency(), "tax-groups" => new TaxGroup(),
            _ => throw new ArgumentOutOfRangeException(nameof(kind))
        };
        ApplyDefinition(entity, request, code);
        return entity;
    }

    private static void ApplyDefinition(DefinitionEntity entity, SaveDefinitionRequest request, string code)
    {
        entity.Code = code; entity.Name = request.Name.Trim(); entity.Description = request.Description?.Trim();
        entity.IsActive = request.IsActive; entity.IsDefault = request.IsDefault; entity.DisplayOrder = request.DisplayOrder;
        if (entity is PaymentTerm payment) { payment.DueDays = request.DueDays ?? 0; payment.DiscountDays = request.DiscountDays; payment.DiscountRate = request.DiscountRate; }
        if (entity is Currency currency) { currency.IsoCode = (request.IsoCode ?? code).Trim().ToUpperInvariant(); currency.Symbol = request.Symbol?.Trim() ?? string.Empty; currency.DecimalPlaces = Math.Clamp(request.DecimalPlaces ?? 2, 0, 6); }
        if (entity is TaxGroup tax) tax.IsTaxExempt = request.IsTaxExempt ?? false;
    }

    private static async Task ClearDefaults<T>(IGenericRepository<T> repository, long? exceptId, CancellationToken ct) where T : DefinitionEntity
    {
        var defaults = await repository.Query(tracking: true).Where(x => x.IsDefault && (!exceptId.HasValue || x.Id != exceptId.Value)).ToListAsync(ct);
        foreach (var item in defaults) item.IsDefault = false;
    }
}


