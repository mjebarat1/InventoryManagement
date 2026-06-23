using InventoryManagement.Domain.Shared;
using InventoryManagement.Domain.Shared.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagement.Domain.StockMovement
{
    public class InventoryMovement : StockMovement
    {
        private InventoryMovement()
        {
            // EF Core
        }

        private InventoryMovement(Guid articleId)
            : base(articleId)
        {
        }

        public static InventoryMovement Create(Guid articleId, IReadOnlyCollection<StockInventoryAdjustment> adjustments)
        {
            var movement = new InventoryMovement(articleId);

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

            return movement;
        }

    }
}
