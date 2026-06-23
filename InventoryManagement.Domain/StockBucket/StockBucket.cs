using InventoryManagement.Domain.Articles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InventoryManagement.Domain.Shared.ValueObjects;

namespace InventoryManagement.Domain.StockBucket
{
    public abstract class StockBucket
    {
        public Guid Id { get; private set; }
        public Guid ArticleId { get; private set; }
        public StockBucketReference Reference { get; private set; }
        public DateTime CreatedAt { get; private set; }

        public Article? Article { get; private set; }

        protected StockBucket()
        {
            // EF Core
        }

        protected StockBucket(Guid articleId, StockBucketReference reference)
        {
            Id = Guid.NewGuid();
            ArticleId = articleId;
            Reference = reference;
            CreatedAt = DateTime.UtcNow;
            Article = null; 
        }


        public abstract bool IsSellable(DateOnly today);

    }
}
