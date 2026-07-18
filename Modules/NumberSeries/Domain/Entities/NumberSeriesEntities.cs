using verii_metivon_api.Core.Domain;

namespace verii_metivon_api.Modules.NumberSeries.Domain.Entities;

public enum NumberSeriesScopeType { Global = 0, Branch = 1, Warehouse = 2 }
public enum NumberSeriesResetPeriod { Never = 0, Yearly = 1, Monthly = 2, Daily = 3 }
public enum NumberSeriesUsageStatus { Reserved = 0, Used = 1, Cancelled = 2, Voided = 3 }

public sealed class DocumentNumberSeries : Entity
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Module { get; set; } = string.Empty;
    public string Reference { get; set; } = string.Empty;
    public NumberSeriesScopeType ScopeType { get; set; }
    public long? BranchId { get; set; }
    public long? WarehouseId { get; set; }
    public string Format { get; set; } = "{SERIES}{YYYY}{NUMBER:9}";
    public NumberSeriesResetPeriod ResetPeriod { get; set; } = NumberSeriesResetPeriod.Yearly;
    public long StartingNumber { get; set; } = 1;
    public int IncrementBy { get; set; } = 1;
    public long MaximumNumber { get; set; } = 999999999;
    public bool IsGibCompliant { get; set; }
    public bool AllowManual { get; set; }
    public bool IsContinuous { get; set; }
    public int ReservationTimeoutMinutes { get; set; } = 30;
    public bool IsDefault { get; set; }
    public int Priority { get; set; } = 100;
    public DateOnly? ValidFrom { get; set; }
    public DateOnly? ValidTo { get; set; }
    public bool IsActive { get; set; } = true;
    public byte[] RowVersion { get; set; } = [];
    public ICollection<NumberSeriesAssignment> Assignments { get; set; } = new List<NumberSeriesAssignment>();
    public ICollection<NumberSeriesCounter> Counters { get; set; } = new List<NumberSeriesCounter>();
}

public sealed class NumberSeriesAssignment : Entity
{
    public long NumberSeriesId { get; set; }
    public DocumentNumberSeries NumberSeries { get; set; } = null!;
    public long? BranchId { get; set; }
    public long? WarehouseId { get; set; }
    public long? UserId { get; set; }
    public long? BusinessPartnerId { get; set; }
    public string? Channel { get; set; }
    public string? Scenario { get; set; }
    public int Priority { get; set; } = 100;
    public bool IsDefault { get; set; }
    public bool IsActive { get; set; } = true;
}

public sealed class NumberSeriesCounter : Entity
{
    public long NumberSeriesId { get; set; }
    public DocumentNumberSeries NumberSeries { get; set; } = null!;
    public string PeriodKey { get; set; } = "ALL";
    public long NextNumber { get; set; } = 1;
    public byte[] RowVersion { get; set; } = [];
}

public sealed class NumberSeriesUsage : Entity
{
    public long NumberSeriesId { get; set; }
    public DocumentNumberSeries NumberSeries { get; set; } = null!;
    public long NumberSeriesCounterId { get; set; }
    public NumberSeriesCounter NumberSeriesCounter { get; set; } = null!;
    public string PeriodKey { get; set; } = string.Empty;
    public long SequenceNumber { get; set; }
    public string DocumentNumber { get; set; } = string.Empty;
    public NumberSeriesUsageStatus Status { get; set; } = NumberSeriesUsageStatus.Reserved;
    public string DocumentType { get; set; } = string.Empty;
    public long? DocumentId { get; set; }
    public long? UserId { get; set; }
    public DateTime ReservedAt { get; set; } = DateTime.UtcNow;
    public DateTime ReservationExpiresAt { get; set; }
    public DateTime? UsedAt { get; set; }
    public DateTime? CancelledAt { get; set; }
    public DateTime? RecycledAt { get; set; }
    public string? CancellationReason { get; set; }
}
