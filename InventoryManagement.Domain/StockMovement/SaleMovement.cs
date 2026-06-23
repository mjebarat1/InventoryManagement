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
    public class SaleMovement : StockMovement
    {
        public Money UnitPriceExcludingTax { get; private set; }
        public Money UnitPriceIncludingTax { get; private set; }
        public VatRate VatRate { get; private set; }

        private SaleMovement()
        {
            // EF Core
        }

        private SaleMovement(Guid articleId,Money unitPriceExcludingTax, Money unitPriceIncludingTax, VatRate VatRate)
            : base(articleId)
        {
            UnitPriceExcludingTax = unitPriceExcludingTax;
            UnitPriceIncludingTax = unitPriceIncludingTax;
            this.VatRate = VatRate;
        }


        public static SaleMovement Create(
            Guid articleId,
            Guid stockBucketId,
            Money unitPriceExcludingTax,
            Money unitPriceIncludingTax,
            VatRate vatRate,
            IReadOnlyCollection<StockConsumption> consumptions)
        {
            var movement = new SaleMovement(articleId, unitPriceExcludingTax, unitPriceIncludingTax, vatRate);

            foreach (var consumption in consumptions)
            {
                movement.AddLine(StockMovementLine.CreateConsumptionLine(
                    stockMovementId: movement.Id,
                    stockBucketId: consumption.StockBucketId,
                    currentQuantity: consumption.CurrentQuantity,
                    consumedQuantity: consumption.ConsumedQuantity));
            }

            return movement;
        }

        public void ConsumeBucket(
            Guid stockBucketId,
            Quantity currentQuantity,
            Quantity consumedQuantity)
        {
            var quantityAfter = currentQuantity - consumedQuantity;

            AddLine(StockMovementLine.CreateConsumptionLine(
                stockMovementId: Id,
                stockBucketId: stockBucketId,
                currentQuantity: currentQuantity,
                consumedQuantity: consumedQuantity));
        }
    }
}
