using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using verii_metivon_api.Modules.Accounting.Domain.Entities;

namespace verii_metivon_api.Modules.Accounting.Infrastructure.Persistence.Configurations;

public sealed class LedgerAccountConfiguration : IEntityTypeConfiguration<LedgerAccount>
{
    public void Configure(EntityTypeBuilder<LedgerAccount> b) { b.ToTable("RII_LEDGER_ACCOUNTS"); b.HasIndex(x => x.Code).IsUnique(); b.Property(x => x.Code).HasMaxLength(30).IsRequired(); b.HasOne(x => x.Parent).WithMany().HasForeignKey(x => x.ParentId).OnDelete(DeleteBehavior.Restrict); }
}
public sealed class FiscalPeriodConfiguration : IEntityTypeConfiguration<FiscalPeriod>
{
    public void Configure(EntityTypeBuilder<FiscalPeriod> b) { b.ToTable("RII_FISCAL_PERIODS"); b.HasQueryFilter(x => !x.IsDeleted); b.HasIndex(x => x.Code).IsUnique().HasFilter("[IsDeleted] = 0"); b.Property(x => x.Code).HasMaxLength(30).IsRequired(); b.Property(x => x.Name).HasMaxLength(150).IsRequired(); }
}
public sealed class InventoryPostingProfileConfiguration : IEntityTypeConfiguration<InventoryPostingProfile>
{
    public void Configure(EntityTypeBuilder<InventoryPostingProfile> b) { b.ToTable("RII_INVENTORY_POSTING_PROFILES"); b.HasQueryFilter(x => !x.IsDeleted); b.HasIndex(x => new { x.Code, x.MovementType }).IsUnique().HasFilter("[IsDeleted] = 0"); b.Property(x => x.Code).HasMaxLength(30).IsRequired(); b.Property(x => x.Name).HasMaxLength(150).IsRequired(); b.HasOne(x => x.DebitAccount).WithMany().HasForeignKey(x => x.DebitAccountId).OnDelete(DeleteBehavior.Restrict); b.HasOne(x => x.CreditAccount).WithMany().HasForeignKey(x => x.CreditAccountId).OnDelete(DeleteBehavior.Restrict); }
}
public sealed class JournalEntryConfiguration : IEntityTypeConfiguration<JournalEntry>
{
    public void Configure(EntityTypeBuilder<JournalEntry> b) { b.ToTable("RII_JOURNAL_ENTRIES"); b.HasIndex(x => x.JournalNumber).IsUnique(); b.HasIndex(x => new { x.SourceType, x.SourceId }); b.Property(x => x.ExchangeRate).HasPrecision(18, 8); b.HasOne(x => x.ReversalOf).WithMany().HasForeignKey(x => x.ReversalOfId).OnDelete(DeleteBehavior.Restrict); }
}
public sealed class JournalEntryLineConfiguration : IEntityTypeConfiguration<JournalEntryLine>
{
    public void Configure(EntityTypeBuilder<JournalEntryLine> b) { b.ToTable("RII_JOURNAL_ENTRY_LINES"); b.HasIndex(x => new { x.JournalEntryId, x.LineNumber }).IsUnique(); foreach (var property in new[] { nameof(JournalEntryLine.Debit), nameof(JournalEntryLine.Credit), nameof(JournalEntryLine.ForeignDebit), nameof(JournalEntryLine.ForeignCredit) }) b.Property(property).HasPrecision(24, 8); b.HasOne(x => x.JournalEntry).WithMany(x => x.Lines).HasForeignKey(x => x.JournalEntryId).OnDelete(DeleteBehavior.Restrict); }
}
public sealed class InventoryCostCloseConfiguration : IEntityTypeConfiguration<InventoryCostClose>
{
    public void Configure(EntityTypeBuilder<InventoryCostClose> b) { b.ToTable("RII_INVENTORY_COST_CLOSES"); b.HasIndex(x => x.CloseNumber).IsUnique(); b.Property(x => x.AdjustmentAmount).HasPrecision(24, 8); }
}
public sealed class AccountingParameterSettingsConfiguration : IEntityTypeConfiguration<AccountingParameterSettings>
{
    public void Configure(EntityTypeBuilder<AccountingParameterSettings> b)
    {
        b.ToTable("RII_ACCOUNTING_PARAMETERS", table =>
        {
            table.HasCheckConstraint("CK_RII_ACCOUNTING_PARAMETERS_DECIMALS", "[MonetaryDecimalPlaces] >= 2 AND [MonetaryDecimalPlaces] <= 8 AND [CostDecimalPlaces] >= 2 AND [CostDecimalPlaces] <= 8");
            table.HasCheckConstraint("CK_RII_ACCOUNTING_PARAMETERS_VALUES", "[DefaultExchangeRate] > 0 AND [JournalBalanceTolerance] >= 0 AND [CostCloseTolerance] >= 0");
        });
        b.HasQueryFilter(x => !x.IsDeleted);
        b.HasIndex(x => new { x.BranchId, x.WarehouseId }).IsUnique().HasFilter("[IsDeleted] = 0");
        b.Property(x => x.DefaultCurrencyCode).HasMaxLength(3).IsUnicode(false).IsRequired();
        b.Property(x => x.DefaultExchangeRate).HasPrecision(18, 8);
        b.Property(x => x.JournalBalanceTolerance).HasPrecision(24, 8);
        b.Property(x => x.CostCloseTolerance).HasPrecision(24, 8);
        b.Property(x => x.RowVersion).IsRowVersion();
        b.HasOne(x => x.Branch).WithMany().HasForeignKey(x => x.BranchId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(x => x.Warehouse).WithMany().HasForeignKey(x => x.WarehouseId).OnDelete(DeleteBehavior.Restrict);
    }
}
