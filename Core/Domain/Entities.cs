namespace verii_metivon_api.Core.Domain;

public abstract class Entity
{
    public long Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public long? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public long? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public long? DeletedBy { get; set; }
}

public abstract class DefinitionEntity : Entity
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsDefault { get; set; }
    public int DisplayOrder { get; set; }
}

public sealed class BusinessPartnerType : DefinitionEntity { }
public sealed class CustomerGroup : DefinitionEntity { }

public sealed class PaymentTerm : DefinitionEntity
{
    public int DueDays { get; set; }
    public int? DiscountDays { get; set; }
    public decimal? DiscountRate { get; set; }
}

public sealed class Currency : DefinitionEntity
{
    public string IsoCode { get; set; } = "TRY";
    public string Symbol { get; set; } = "₺";
    public int DecimalPlaces { get; set; } = 2;
}

public sealed class TaxGroup : DefinitionEntity
{
    public bool IsTaxExempt { get; set; }
}

public sealed class BusinessPartner : Entity
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? LegalName { get; set; }
    public long BranchId { get; set; }
    public Branch Branch { get; set; } = null!;
    public long BusinessPartnerTypeId { get; set; }
    public BusinessPartnerType BusinessPartnerType { get; set; } = null!;
    public long? CustomerGroupId { get; set; }
    public CustomerGroup? CustomerGroup { get; set; }
    public long PaymentTermId { get; set; }
    public PaymentTerm PaymentTerm { get; set; } = null!;
    public long CurrencyId { get; set; }
    public Currency Currency { get; set; } = null!;
    public long TaxGroupId { get; set; }
    public TaxGroup TaxGroup { get; set; } = null!;
    public string? TaxOffice { get; set; }
    public string? TaxNumber { get; set; }
    public string? NationalIdentityNumber { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? MobilePhone { get; set; }
    public string? Website { get; set; }
    public decimal CreditLimit { get; set; }
    public bool HasUnlimitedCredit { get; set; }
    public bool IsActive { get; set; } = true;
    public string? Notes { get; set; }
}
