using verii_metivon_api.Modules.Procurement.Application.Services;
using Xunit;

namespace verii_metivon_api.ArchitectureTests.Procurement;

public sealed class PurchaseOrderAmountCalculatorTests
{
    [Fact]
    public void Gross_document_cost_carries_discount_and_tax_consistently()
    {
        var result = PurchaseOrderAmountCalculator.Calculate(10m, 100m, 10m, 20m);

        Assert.Equal(1000m, result.GrossAmount);
        Assert.Equal(100m, result.DiscountAmount);
        Assert.Equal(900m, result.NetAmount);
        Assert.Equal(180m, result.TaxAmount);
        Assert.Equal(1080m, result.GrandTotal);
        Assert.Equal(108m, result.GrossUnitCost);
    }
}
