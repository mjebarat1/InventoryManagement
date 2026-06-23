using InventoryManagement.Domain.Shared.Exceptions;
using InventoryManagement.Domain.Shared.ValueObjects;
using InventoryManagement.Domain.StockBucket;

namespace InventoryManagement.Test;

public sealed class StockBucketReferenceTests
{
    [Fact]
    public void Create_AcceptsExpectedFormat()
    {
        var reference = StockBucketReference.Create("ref-lot-0000000000042");

        Assert.Equal("ref-lot-0000000000042", reference.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData("ref-lot-1234")]
    [InlineData("REF-LOT-0000000000042")]
    [InlineData("ref-lot-000000000004x")]
    public void Create_RejectsInvalidFormat(string value)
    {
        Assert.Throws<BusinessRuleException>(() => StockBucketReference.Create(value));
    }
}
