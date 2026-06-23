using InventoryManagement.Domain.Articles;
using InventoryManagement.Domain.StockBucket;
using InventoryManagement.Domain.StockMovement;

namespace InventoryManagement.Application.Ports.Out;

public interface IArticleStockReadRepository
{
    Task<ArticleStockSnapshot?> GetByArticleIdAsync(
        Guid articleId,
        CancellationToken cancellationToken = default);
}

public sealed record ArticleStockSnapshot(
    Article Article,
    IReadOnlyCollection<StockBucket> Buckets,
    IReadOnlyCollection<StockMovement> Movements);
