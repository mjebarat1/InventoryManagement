using InventoryManagement.Domain.Articles;
using InventoryManagement.Domain.Shared;
using InventoryManagement.Domain.Shared.Exceptions;
using InventoryManagement.Domain.Shared.ValueObjects;
using InventoryManagement.Domain.StockBucket;

namespace InventoryManagement.Domain.StockMovement;

public sealed class StockSaleAllocator
{
    public IReadOnlyCollection<StockConsumption> Allocate(
        Article article,
        IReadOnlyCollection<StockBucketAvailability> availabilities,
        Quantity requestedQuantity,
        DateOnly today)
    {
        var eligible = GetEligibleBuckets(article, availabilities, today)
            .Where(item => item.CurrentQuantity.Value > 0)
            .ToArray();
        var availableQuantity = eligible.Sum(item => item.CurrentQuantity.Value);

        if (availableQuantity < requestedQuantity.Value)
        {
            throw new BusinessRuleException(
                $"Stock vendable insuffisant. Disponible : {availableQuantity}, demandé : {requestedQuantity.Value}.");
        }

        var remaining = requestedQuantity.Value;
        var consumptions = new List<StockConsumption>();

        foreach (var availability in eligible)
        {
            if (remaining == 0)
                break;

            var consumed = Math.Min(availability.CurrentQuantity.Value, remaining);
            consumptions.Add(new StockConsumption(
                availability.Bucket.Id,
                availability.CurrentQuantity,
                Quantity.CreatePositive(consumed)));
            remaining -= consumed;
        }

        return consumptions;
    }

    private static IEnumerable<StockBucketAvailability> GetEligibleBuckets(
        Article article,
        IEnumerable<StockBucketAvailability> availabilities,
        DateOnly today)
    {
        return article switch
        {
            FoodArticle => availabilities
                .Where(item => item.Bucket is FoodStockBucket && item.Bucket.IsSellable(today))
                .OrderBy(item => ((FoodStockBucket)item.Bucket).ExpirationDate)
                .ThenBy(item => item.Bucket.CreatedAt),
            NonFoodArticle => availabilities
                .Where(item => item.Bucket is NonFoodStockBucket && item.Bucket.IsSellable(today))
                .OrderBy(item => item.Bucket.CreatedAt),
            _ => throw new BusinessRuleException("Type d'article inconnu.")
        };
    }
}
