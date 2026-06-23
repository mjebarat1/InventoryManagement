using InventoryManagement.Domain.Shared.ValueObjects;

namespace InventoryManagement.Domain.StockBucket
{
    public sealed class FoodStockBucket : StockBucket
    {
        public DateOnly ExpirationDate { get; private set; }

        private FoodStockBucket()
        {
            // EF Core
        }

        private FoodStockBucket(
            Guid articleId,
            StockBucketReference reference,
            DateOnly expirationDate)
            : base(articleId, reference)
        {
            ExpirationDate = expirationDate;
        }

        public static FoodStockBucket Create(
            Guid articleId,
            StockBucketReference reference,
            DateOnly expirationDate)
        {
            return new FoodStockBucket(articleId, reference, expirationDate);
        }

        public override bool IsSellable(DateOnly today)
        {
            return ExpirationDate >= today;
        }
    }
}
