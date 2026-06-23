using InventoryManagement.Domain.StockBucket;
using InventoryManagement.Domain.Shared.ValueObjects;

namespace InventoryManagement.Application.Ports.Out;

public interface IStockBucketRepository
{
    Task<bool> ExistsByReferenceAsync(
        StockBucketReference reference,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<StockBucketQuantitySnapshot>> SearchByReferencePrefixAsync(
        Guid articleId,
        string referencePrefix,
        int maximumResults,
        CancellationToken cancellationToken = default);
}

public sealed record StockBucketQuantitySnapshot(StockBucket Bucket, int PhysicalQuantity);
