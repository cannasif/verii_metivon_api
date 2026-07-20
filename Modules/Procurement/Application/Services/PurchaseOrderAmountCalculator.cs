namespace verii_metivon_api.Modules.Procurement.Application.Services;

public sealed record PurchaseOrderLineAmounts(
    decimal GrossAmount,
    decimal DiscountAmount,
    decimal NetAmount,
    decimal TaxAmount,
    decimal GrandTotal,
    decimal GrossUnitCost);

public static class PurchaseOrderAmountCalculator
{
    public static PurchaseOrderLineAmounts Calculate(
        decimal quantity,
        decimal unitPrice,
        decimal discountRate,
        decimal taxRate)
    {
        if (quantity <= 0) throw new ArgumentOutOfRangeException(nameof(quantity));

        var gross = quantity * unitPrice;
        var discount = gross * discountRate / 100m;
        var net = gross - discount;
        var tax = net * taxRate / 100m;
        var grandTotal = net + tax;
        return new(gross, discount, net, tax, grandTotal, grandTotal / quantity);
    }

    public static decimal GrossUnitCost(decimal quantity, decimal lineTotal) =>
        quantity <= 0 ? 0 : lineTotal / quantity;
}
