using InventoryManagement.Application.Ports.Out;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Infrastructure.Persistence;

internal sealed class ArticleStockReadRepository : IArticleStockReadRepository
{
    private readonly StockDbContext _context;

    public ArticleStockReadRepository(StockDbContext context)
    {
        _context = context;
    }

    public async Task<ArticleStockSnapshot?> GetByArticleIdAsync(
        Guid articleId,
        CancellationToken cancellationToken = default)
    {
        var article = await _context.Articles
            .AsNoTracking()
            .SingleOrDefaultAsync(item => item.Id == articleId, cancellationToken);

        if (article is null)
            return null;

        var buckets = await _context.StockBuckets
            .AsNoTracking()
            .Where(bucket => bucket.ArticleId == articleId)
            .ToListAsync(cancellationToken);

        var movements = await _context.StockMovements
            .AsNoTracking()
            .Where(movement => movement.ArticleId == articleId)
            .Include(movement => movement.Lines)
                .ThenInclude(line => line.StockBucket)
            .AsSplitQuery()
            .ToListAsync(cancellationToken);

        return new ArticleStockSnapshot(article, buckets, movements);
    }
}
