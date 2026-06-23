using InventoryManagement.Domain.Shared;
using InventoryManagement.Domain.Shared.ValueObjects;
using InventoryManagement.Domain.Shared.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagement.Domain.StockMovement
{
    public class InventoryMovement : StockMovement
    {
        public string? Comment { get; private set; }

        private InventoryMovement()
        {
            // EF Core
        }

        private InventoryMovement(Guid articleId, string? comment)
            : base(articleId)
        {
            Comment = string.IsNullOrWhiteSpace(comment) ? null : comment.Trim();
        }

        public static InventoryMovement Create(
            Guid articleId,
            string? comment,
            IReadOnlyCollection<StockInventoryAdjustment> adjustments)
        {
            if (adjustments is null || adjustments.Count == 0)
                throw new BusinessRuleException("Au moins un écart d'inventaire est obligatoire.");

            var movement = new InventoryMovement(articleId, comment);

            foreach (var adjustment in adjustments)
            {
                if (adjustment.CurrentQuantity.Value == adjustment.CountedQuantity.Value)
                {
                    continue;
                }

                movement.AddLine(StockMovementLine.CreateInventoryAdjustmentLine(
                    stockMovementId: movement.Id,
                    stockBucketId: adjustment.StockBucketId,
                    currentQuantity: adjustment.CurrentQuantity,
                    countedQuantity: adjustment.CountedQuantity));
            }

            if (movement.Lines.Count == 0)
                throw new BusinessRuleException("Aucun écart d'inventaire n'a été constaté.");

            return movement;
        }

    }
}
