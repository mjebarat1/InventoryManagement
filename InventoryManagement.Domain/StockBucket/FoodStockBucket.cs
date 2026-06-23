using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagement.Domain.StockBucket
{
    public sealed class FoodStockBucket : StockBucket
    {
        public DateOnly ExpirationDate { get; private set; }

        private FoodStockBucket()
        {
            // EF Core
        }

        private FoodStockBucket(Guid articleId, DateOnly expirationDate)
            : base(articleId)
        {
            ExpirationDate = expirationDate;
        }

        public static FoodStockBucket Create(Guid articleId, DateOnly expirationDate)
        {
            return new FoodStockBucket(articleId, expirationDate);
        }

        public override bool IsSellable(DateOnly today)
        {
            return ExpirationDate >= today;
        }
    }
}
