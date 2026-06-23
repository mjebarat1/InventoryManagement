using InventoryManagement.Domain.Shared.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagement.Domain.StockMovement
{
    public sealed class StockMovementLine
    {
        public Guid Id { get; private set; }
        public Guid StockMovementId { get; private set; }
        public Guid StockBucketId { get; private set; }

        public int QuantityDelta { get; private set; }

        // just epour l'historique
        public Quantity? QuantityBefore { get; private set; }
        public Quantity? QuantityAfter { get; private set; }

        public StockBucket.StockBucket? StockBucket  { get; private set; }

        public StockMovement? StockMovement { get; set;  }


        private StockMovementLine()
        {
            // EF Core
        }

        private StockMovementLine(
            Guid stockMovementId,
            Guid stockBucketId,
            int quantityDelta,
            Quantity? quantityBefore,
            Quantity? quantityAfter)
        {
            Id = Guid.NewGuid();
            StockMovementId = stockMovementId;
            StockBucketId = stockBucketId;
            QuantityDelta = quantityDelta;
            QuantityBefore = quantityBefore;
            QuantityAfter = quantityAfter;
        }

        public static StockMovementLine CreateSupplyLine(
            Guid stockMovementId,
            Guid stockBucketId,
            Quantity suppliedQuantity)
        {
            return new StockMovementLine(
                stockMovementId,
                stockBucketId,
                suppliedQuantity.Value,
                Quantity.Create(0),
                suppliedQuantity);
        }

        public static StockMovementLine CreateConsumptionLine(
            Guid stockMovementId,
            Guid stockBucketId,
            Quantity currentQuantity,
            Quantity consumedQuantity)
        {
            var quantityAfter = currentQuantity - consumedQuantity;

            return new StockMovementLine(
                stockMovementId,
                stockBucketId,
                -consumedQuantity.Value,
                currentQuantity,
                quantityAfter);
        }

        public static StockMovementLine CreateInventoryAdjustmentLine(
            Guid stockMovementId,
            Guid stockBucketId,
            Quantity currentQuantity,
            Quantity countedQuantity)
        {
            var delta = countedQuantity.Value - currentQuantity.Value;

            return new StockMovementLine(
                stockMovementId,
                stockBucketId,
                delta,
                currentQuantity,
                countedQuantity);
        }

    }
}
