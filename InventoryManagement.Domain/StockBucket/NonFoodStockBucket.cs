using InventoryManagement.Domain.Articles;
using InventoryManagement.Domain.Shared.ValueObjects;

namespace InventoryManagement.Domain.StockBucket
{
    public sealed class NonFoodStockBucket : StockBucket
    {
        public PackagingLevel PackagingLevel { get; private set; }

        private NonFoodStockBucket()
        {
            // EF Core
        }

        private NonFoodStockBucket(
            Guid articleId,
            StockBucketReference reference,
            PackagingLevel packagingLevel)
            : base(articleId, reference)
        {
            PackagingLevel = packagingLevel;
        }

        public static NonFoodStockBucket Create(
            Guid articleId,
            StockBucketReference reference,
            PackagingLevel packagingLevel)
        {
            return new NonFoodStockBucket(articleId, reference, packagingLevel);
        }

        public override bool IsSellable(DateOnly today)
        {
            return PackagingLevel is PackagingLevel.New or PackagingLevel.Refurbished;
        }
    }
}
