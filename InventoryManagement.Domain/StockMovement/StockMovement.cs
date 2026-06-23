using InventoryManagement.Domain.Articles;
using InventoryManagement.Domain.Shared;
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
        private readonly List<StockMovementLine> _lines = new();

        public Guid Id { get; private set; }
        public Guid ArticleId { get; private set; }
        public DateTime CreatedAt { get; private set; }

        public IReadOnlyCollection<StockMovementLine> Lines => _lines.AsReadOnly();

        public Article? Article { get; private set; }


        protected StockMovement()
        {
            // EF core
        }

        protected StockMovement(Guid articleId)
        {
            if (articleId == Guid.Empty)
                throw new BusinessRuleException(DomainErrorCodes.ArticleRequired);

            Id = Guid.NewGuid();
            ArticleId = articleId;
            CreatedAt = DateTime.UtcNow;
        }


        protected void AddLine(StockMovementLine line)
        {
            _lines.Add(line);
        }

    }
}
