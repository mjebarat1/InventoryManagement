using InventoryManagement.Domain.Articles;
using InventoryManagement.Domain.Shared.Exceptions;
using InventoryManagement.Domain.Shared.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagement.Domain.StockMovement
{
    public abstract class StockMovement
    {
        public Guid Id { get; private set; }
        public Guid ArticleId { get; private set; }
        public Quantity Quantity { get; protected set; }
        public DateTime CreatedAt { get; private set; }

        public Article? Article { get; private set; }


        protected StockMovement()
        {
            // EF core
        }


        protected StockMovement(Guid articleId, Quantity quantity)
        {
            if (articleId == Guid.Empty)
                throw new BusinessRuleException("L'article est obligatoire.");

            Id = Guid.NewGuid();
            ArticleId = articleId;
            Quantity = quantity;
            CreatedAt = DateTime.UtcNow;
        }

        public abstract Quantity ApplyTo(Quantity currentStock);
    }
}
