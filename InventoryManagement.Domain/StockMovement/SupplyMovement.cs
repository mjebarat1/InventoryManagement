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
    public sealed class SupplyMovement : StockMovement
    {

        private SupplyMovement()
        {
            // EF Core
        }

        private SupplyMovement(Guid articleId)
            : base(articleId)
        {
        }

        public static SupplyMovement Create(
            Guid articleId,
            Guid stockBucketId,
            Quantity suppliedQuantity)
        {
            var movement = new SupplyMovement(articleId);

            movement.AddLine(StockMovementLine.CreateSupplyLine(
                stockMovementId: movement.Id,
                stockBucketId: stockBucketId,
                suppliedQuantity: suppliedQuantity));

            return movement;
        }

    }
}
