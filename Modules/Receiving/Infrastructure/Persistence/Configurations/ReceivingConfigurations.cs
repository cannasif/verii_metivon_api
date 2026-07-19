using Microsoft.EntityFrameworkCore;using Microsoft.EntityFrameworkCore.Metadata.Builders;using verii_metivon_api.Modules.Receiving.Domain.Entities;
namespace verii_metivon_api.Modules.Receiving.Infrastructure.Persistence.Configurations;
public sealed class GoodsReceiptConfiguration:IEntityTypeConfiguration<GoodsReceipt>{public void Configure(EntityTypeBuilder<GoodsReceipt>b){b.ToTable("RII_GOODS_RECEIPTS");b.HasIndex(x=>x.ReceiptNumber).IsUnique();b.Property(x=>x.ReceiptNumber).HasMaxLength(40).IsRequired();b.HasOne(x=>x.PurchaseOrder).WithMany().HasForeignKey(x=>x.PurchaseOrderId).OnDelete(DeleteBehavior.Restrict);b.HasOne<verii_metivon_api.Modules.TradeOperations.Domain.Entities.TradeDossier>().WithMany().HasForeignKey(x=>x.TradeDossierId).OnDelete(DeleteBehavior.Restrict);}}
public sealed class ReceivingParameterSettingsConfiguration:IEntityTypeConfiguration<ReceivingParameterSettings>
{
    public void Configure(EntityTypeBuilder<ReceivingParameterSettings>b)
    {
        b.ToTable("RII_RECEIVING_PARAMETERS",t=>
        {
            t.HasCheckConstraint("CK_RII_RECEIVING_PARAMETERS_TOLERANCE","[OverDeliveryTolerancePercent] >= 0 AND [OverDeliveryTolerancePercent] <= 100 AND [UnderDeliveryTolerancePercent] >= 0 AND [UnderDeliveryTolerancePercent] <= 100");
            t.HasCheckConstraint("CK_RII_RECEIVING_PARAMETERS_SHELF_LIFE","[MinimumRemainingShelfLifeDays] >= 0");
            t.HasCheckConstraint("CK_RII_RECEIVING_PARAMETERS_LABEL_COPIES","[DefaultLabelCopies] >= 1 AND [DefaultLabelCopies] <= 100");
        });
        b.HasKey(x=>x.Id);b.HasQueryFilter(x=>!x.IsDeleted);
        b.HasIndex(x=>new{x.BranchId,x.WarehouseId}).IsUnique().HasFilter("[IsDeleted] = 0");
        b.Property(x=>x.OverDeliveryTolerancePercent).HasPrecision(7,4);b.Property(x=>x.UnderDeliveryTolerancePercent).HasPrecision(7,4);
        b.Property(x=>x.InventoryCurrencyCode).HasMaxLength(3).IsUnicode(false).IsRequired();b.Property(x=>x.RowVersion).IsRowVersion();b.HasOne(x=>x.InventoryCurrency).WithMany().HasForeignKey(x=>x.InventoryCurrencyId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(x=>x.Branch).WithMany().HasForeignKey(x=>x.BranchId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(x=>x.Warehouse).WithMany().HasForeignKey(x=>x.WarehouseId).OnDelete(DeleteBehavior.Restrict);
    }
}
public sealed class GoodsReceiptLineConfiguration:IEntityTypeConfiguration<GoodsReceiptLine>{public void Configure(EntityTypeBuilder<GoodsReceiptLine>b){b.ToTable("RII_GOODS_RECEIPT_LINES");b.HasIndex(x=>new{x.GoodsReceiptId,x.LineNumber}).IsUnique();foreach(var p in new[]{nameof(GoodsReceiptLine.ExpectedQuantity),nameof(GoodsReceiptLine.ReceivedQuantity),nameof(GoodsReceiptLine.AcceptedQuantity),nameof(GoodsReceiptLine.RejectedQuantity),nameof(GoodsReceiptLine.UnitCost)})b.Property(p).HasPrecision(24,8);b.HasOne(x=>x.GoodsReceipt).WithMany(x=>x.Lines).HasForeignKey(x=>x.GoodsReceiptId).OnDelete(DeleteBehavior.Restrict);b.HasOne(x=>x.PurchaseOrderLine).WithMany().HasForeignKey(x=>x.PurchaseOrderLineId).OnDelete(DeleteBehavior.Restrict);}}
public sealed class GoodsReceiptSerialConfiguration:IEntityTypeConfiguration<GoodsReceiptSerial>{public void Configure(EntityTypeBuilder<GoodsReceiptSerial>b){b.ToTable("RII_GOODS_RECEIPT_SERIALS");b.HasIndex(x=>new{x.GoodsReceiptLineId,x.SerialNumber}).IsUnique();b.Property(x=>x.SerialNumber).HasMaxLength(120).IsRequired();b.HasOne(x=>x.GoodsReceiptLine).WithMany(x=>x.Serials).HasForeignKey(x=>x.GoodsReceiptLineId).OnDelete(DeleteBehavior.Restrict);}}
