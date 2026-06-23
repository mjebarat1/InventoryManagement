using InventoryManagement.Application.Ports.Out;
using InventoryManagement.Domain.StockBucket;
using Microsoft.EntityFrameworkCore;
using InventoryManagement.Domain.Shared.ValueObjects;

namespace InventoryManagement.Infrastructure.Persistence;

internal sealed class StockBucketRepository : IStockBucketRepository
{
    private readonly StockDbContext _context;

    public StockBucketRepository(StockDbContext context)
    {
        _context = context;
    }

    public Task<bool> ExistsByReferenceAsync(
        StockBucketReference reference,
        CancellationToken cancellationToken = default) => _context.StockBuckets
        .AnyAsync(bucket => bucket.Reference.Value == reference.Value, cancellationToken);

    public async Task<IReadOnlyCollection<StockBucketQuantitySnapshot>> SearchByReferencePrefixAsync(
        Guid articleId,
        string referencePrefix,
        int maximumResults,
        CancellationToken cancellationToken = default)
    {
        var buckets = await _context.StockBuckets
            .AsNoTracking()
            .Where(bucket => bucket.ArticleId == articleId
                && bucket.Reference.Value.StartsWith(referencePrefix))
            .OrderBy(bucket => bucket.Reference.Value)
            .Take(maximumResults)
            .ToListAsync(cancellationToken);
        if (buckets.Count == 0)
            return [];

        var bucketIds = buckets.Select(bucket => bucket.Id).ToArray();
        var quantities = await _context.StockMovementLines
            .AsNoTracking()
            .Where(line => bucketIds.Contains(line.StockBucketId))
            .GroupBy(line => line.StockBucketId)
            .Select(group => new { StockBucketId = group.Key, Quantity = group.Sum(line => line.QuantityDelta) })
            .ToDictionaryAsync(item => item.StockBucketId, item => item.Quantity, cancellationToken);

        return buckets
            .Select(bucket => new StockBucketQuantitySnapshot(
                bucket,
                quantities.GetValueOrDefault(bucket.Id)))
            .ToArray();
    }
}
