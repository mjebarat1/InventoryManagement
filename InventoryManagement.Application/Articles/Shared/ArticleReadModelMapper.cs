using InventoryManagement.Application.Ports.Out;
using InventoryManagement.Domain.Articles;
using InventoryManagement.Domain.StockBucket;
using InventoryManagement.Domain.StockMovement;

namespace InventoryManagement.Application.Articles.Shared;

internal static class ArticleReadModelMapper
{
    public static ArticleSummaryResult ToSummary(Article article) => new(
        article.Id,
        article.Reference.Value,
        article.Name,
        GetArticleKind(article),
        article.PriceExcludingTax.Amount,
        GetPrices(article),
        CalculateTotalStock(article.StockMovements));

    public static ArticleDetailsResult ToDetails(ArticleStockSnapshot snapshot, DateOnly today)
    {
        var quantitiesByBucket = snapshot.Movements
            .SelectMany(movement => movement.Lines)
            .GroupBy(line => line.StockBucketId)
            .ToDictionary(group => group.Key, group => group.Sum(line => line.QuantityDelta));

        var buckets = snapshot.Buckets
            .OrderBy(bucket => bucket.CreatedAt)
            .Select(bucket => ToBucket(bucket, quantitiesByBucket.GetValueOrDefault(bucket.Id), today))
            .ToArray();

        var bucketsById = snapshot.Buckets.ToDictionary(bucket => bucket.Id);
        var movements = snapshot.Movements
            .OrderByDescending(movement => movement.CreatedAt)
            .Select(movement => ToMovement(movement, bucketsById))
            .ToArray();

        var article = snapshot.Article;
        return new ArticleDetailsResult(
            article.Id,
            article.Reference.Value,
            article.Name,
            GetArticleKind(article),
            article.PriceExcludingTax.Amount,
            article is FoodArticle foodArticle ? foodArticle.SaleModes : Array.Empty<SaleMode>(),
            GetPrices(article),
            buckets.Sum(bucket => bucket.PhysicalQuantity),
            buckets.Sum(bucket => bucket.SellableQuantity),
            buckets.Where(bucket => bucket.Status is StockBucketStatus.Expired or StockBucketStatus.Unsellable)
                .Sum(bucket => bucket.PhysicalQuantity),
            buckets,
            movements);
    }

    private static ArticleKind GetArticleKind(Article article) => article switch
    {
        FoodArticle => ArticleKind.Food,
        NonFoodArticle => ArticleKind.NonFood,
        _ => throw new InvalidOperationException("Type d'article inconnu.")
    };

    private static ArticleKind GetBucketKind(StockBucket bucket) => bucket switch
    {
        FoodStockBucket => ArticleKind.Food,
        NonFoodStockBucket => ArticleKind.NonFood,
        _ => throw new InvalidOperationException("Type de bucket inconnu.")
    };

    private static IReadOnlyCollection<ArticlePriceResult> GetPrices(Article article)
    {
        if (article is FoodArticle foodArticle)
            return foodArticle.SaleModes.Select(mode => CreatePrice(article, mode, mode)).ToArray();

        return [CreatePrice(article, SaleMode.TakeAway, null)];
    }

    private static ArticlePriceResult CreatePrice(Article article, SaleMode calculationMode, SaleMode? displayedMode)
    {
        var vatRate = article.GetVatRate(calculationMode);
        return new ArticlePriceResult(
            displayedMode,
            vatRate.Value,
            article.GetPriceIncludingTax(calculationMode).Amount);
    }

    private static int CalculateTotalStock(IEnumerable<StockMovement> movements) => movements
        .SelectMany(movement => movement.Lines)
        .Sum(line => line.QuantityDelta);

    private static StockBucketResult ToBucket(StockBucket bucket, int physicalQuantity, DateOnly today)
    {
        var status = GetBucketStatus(bucket, physicalQuantity, today);
        return new StockBucketResult(
            bucket.Id,
            bucket.CreatedAt,
            GetBucketKind(bucket),
            bucket is FoodStockBucket foodBucket ? foodBucket.ExpirationDate : null,
            bucket is NonFoodStockBucket nonFoodBucket ? nonFoodBucket.PackagingLevel : null,
            physicalQuantity,
            status == StockBucketStatus.Sellable ? physicalQuantity : 0,
            status);
    }

    private static StockBucketStatus GetBucketStatus(StockBucket bucket, int physicalQuantity, DateOnly today)
    {
        if (physicalQuantity == 0)
            return StockBucketStatus.Empty;
        if (bucket.IsSellable(today))
            return StockBucketStatus.Sellable;
        return bucket is FoodStockBucket ? StockBucketStatus.Expired : StockBucketStatus.Unsellable;
    }

    private static StockMovementResult ToMovement(
        StockMovement movement,
        IReadOnlyDictionary<Guid, StockBucket> bucketsById)
    {
        var lines = movement.Lines.Select(line =>
        {
            if (!bucketsById.TryGetValue(line.StockBucketId, out var bucket))
                throw new InvalidOperationException($"Bucket '{line.StockBucketId}' introuvable.");

            return new StockMovementLineResult(
                line.Id,
                line.StockBucketId,
                GetBucketKind(bucket),
                bucket is FoodStockBucket foodBucket ? foodBucket.ExpirationDate : null,
                bucket is NonFoodStockBucket nonFoodBucket ? nonFoodBucket.PackagingLevel : null,
                line.QuantityDelta,
                line.QuantityBefore?.Value,
                line.QuantityAfter?.Value);
        }).ToArray();

        return new StockMovementResult(
            movement.Id,
            movement.CreatedAt,
            movement switch
            {
                SupplyMovement => "Supply",
                SaleMovement => "Sale",
                InventoryMovement => "Inventory",
                _ => "Unknown"
            },
            lines.Sum(line => line.QuantityDelta),
            lines);
    }
}
