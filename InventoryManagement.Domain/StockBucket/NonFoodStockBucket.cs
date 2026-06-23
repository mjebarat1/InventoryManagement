using InventoryManagement.Domain.Articles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagement.Domain.StockBucket
{
    public sealed class NonFoodStockBucket : StockBucket
    {
        public PackagingLevel PackagingLevel { get; private set; }

        private NonFoodStockBucket()
        {
            // EF Core
        }

        private NonFoodStockBucket(Guid articleId, PackagingLevel packagingLevel)
            : base(articleId)
        {
            PackagingLevel = packagingLevel;
        }

        public static NonFoodStockBucket Create(Guid articleId, PackagingLevel packagingLevel)
        {
            return new NonFoodStockBucket(articleId, packagingLevel);
        }

        public override bool IsSellable(DateOnly today)
        {
            return PackagingLevel is PackagingLevel.New or PackagingLevel.Refurbished;
        }
    }
}
