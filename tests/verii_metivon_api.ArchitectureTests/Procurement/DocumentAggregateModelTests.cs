using Microsoft.EntityFrameworkCore;
using verii_metivon_api.Core.Persistence;
using verii_metivon_api.Modules.Procurement.Domain.Entities;
using verii_metivon_api.Modules.Receiving.Domain.Entities;
using verii_metivon_api.Modules.Shipping.Domain.Entities;
using verii_metivon_api.Modules.TradeOperations.Domain.Entities;
using Xunit;

namespace verii_metivon_api.ArchitectureTests.Procurement;

public sealed class DocumentAggregateModelTests
{
    private static MetivonDbContext CreateModelContext()
    {
        var options = new DbContextOptionsBuilder<MetivonDbContext>()
            .UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=MetivonModelContract;Trusted_Connection=True")
            .Options;
        return new MetivonDbContext(options);
    }

    [Fact]
    public void Purchase_order_uses_separate_header_and_line_tables_with_optional_location()
    {
        using var db = CreateModelContext();
        var header = db.Model.FindEntityType(typeof(PurchaseOrder));
        var line = db.Model.FindEntityType(typeof(PurchaseOrderLine));

        Assert.Equal("RII_PURCHASE_ORDERS", header?.GetTableName());
        Assert.Equal("RII_PURCHASE_ORDER_LINES", line?.GetTableName());
        Assert.True(line?.FindProperty(nameof(PurchaseOrderLine.StorageLocationId))?.IsNullable);
        Assert.Contains(line!.GetForeignKeys(), key => key.PrincipalEntityType.ClrType == typeof(PurchaseOrder));
    }

    [Fact]
    public void Receipt_and_shipment_aggregates_keep_headers_and_lines_separate()
    {
        using var db = CreateModelContext();

        Assert.Equal("RII_GOODS_RECEIPTS", db.Model.FindEntityType(typeof(GoodsReceipt))?.GetTableName());
        Assert.Equal("RII_GOODS_RECEIPT_LINES", db.Model.FindEntityType(typeof(GoodsReceiptLine))?.GetTableName());
        Assert.Equal("RII_SHIPMENTS", db.Model.FindEntityType(typeof(Shipment))?.GetTableName());
        Assert.Equal("RII_SHIPMENT_LINES", db.Model.FindEntityType(typeof(ShipmentLine))?.GetTableName());
        Assert.Equal("RII_DELIVERY_NOTES", db.Model.FindEntityType(typeof(DeliveryNote))?.GetTableName());
    }

    [Fact]
    public void One_trade_dossier_can_link_many_orders_and_partial_receipts()
    {
        using var db = CreateModelContext();
        var purchaseOrder = db.Model.FindEntityType(typeof(PurchaseOrder))!;
        var goodsReceipt = db.Model.FindEntityType(typeof(GoodsReceipt))!;
        var receiptOrderLink = db.Model.FindEntityType(typeof(GoodsReceiptPurchaseOrder))!;

        var purchaseDossierForeignKey = purchaseOrder.GetForeignKeys().Single(key => key.PrincipalEntityType.ClrType == typeof(TradeDossier));
        var receiptDossierForeignKey = goodsReceipt.GetForeignKeys().Single(key => key.PrincipalEntityType.ClrType == typeof(TradeDossier));

        Assert.False(purchaseDossierForeignKey.IsUnique);
        Assert.False(receiptDossierForeignKey.IsUnique);
        Assert.DoesNotContain(receiptOrderLink.GetIndexes(), index =>
            index.IsUnique && index.Properties.Count == 1 && index.Properties[0].Name == nameof(GoodsReceiptPurchaseOrder.PurchaseOrderId));
        Assert.Contains(receiptOrderLink.GetIndexes(), index =>
            index.IsUnique && index.Properties.Select(property => property.Name).SequenceEqual([
                nameof(GoodsReceiptPurchaseOrder.GoodsReceiptId),
                nameof(GoodsReceiptPurchaseOrder.PurchaseOrderId)
            ]));
    }
}
