using verii_metivon_api.Core.Domain;
namespace verii_metivon_api.Modules.Parameters.Domain.Entities;

public sealed class ErpParameter : Entity
{
    public string Module { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public long? BranchId { get; set; }
    public Branch? Branch { get; set; }
    public string ValueType { get; set; } = "String";
    public string Value { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsEditable { get; set; } = true;
    public byte[] RowVersion { get; set; } = [];
}

public sealed class NumberSequence : Entity
{
    public string Module { get; set; } = string.Empty;
    public string Reference { get; set; } = string.Empty;
    public long? BranchId { get; set; }
    public Branch? Branch { get; set; }
    public string Format { get; set; } = "{NUMBER:6}";
    public long CurrentNumber { get; set; } = 1;
    public int IncrementBy { get; set; } = 1;
    public long MinimumNumber { get; set; } = 1;
    public long MaximumNumber { get; set; } = 999999999;
    public bool IsAutomatic { get; set; } = true;
    public bool AllowManual { get; set; }
    public bool IsContinuous { get; set; }
    public bool IsActive { get; set; } = true;
    public byte[] RowVersion { get; set; } = [];
}
