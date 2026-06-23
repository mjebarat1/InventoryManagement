using InventoryManagement.Domain.Shared;
using InventoryManagement.Domain.Shared.ValueObjects;
using InventoryManagement.Domain.Articles;

namespace InventoryManagement.Domain.StockMovement
{
    public class SaleMovement : StockMovement
    {
        public Money UnitPriceExcludingTax { get; private set; } = null!;
        public Money UnitPriceIncludingTax { get; private set; } = null!;
        public VatRate VatRate { get; private set; } = null!;
        public SaleMode? SaleMode { get; private set; }

        private SaleMovement()
        {
            // EF Core
        }

        private SaleMovement(
            Guid articleId,
            Money unitPriceExcludingTax,
            Money unitPriceIncludingTax,
            VatRate vatRate,
            SaleMode? saleMode)
            : base(articleId)
        {
            UnitPriceExcludingTax = unitPriceExcludingTax;
            UnitPriceIncludingTax = unitPriceIncludingTax;
            VatRate = vatRate;
            SaleMode = saleMode;
        }


        public static SaleMovement Create(
            Guid articleId,
            Money unitPriceExcludingTax,
            Money unitPriceIncludingTax,
            VatRate vatRate,
            SaleMode? saleMode,
            IReadOnlyCollection<StockConsumption> consumptions)
        {
            var movement = new SaleMovement(
                articleId,
                unitPriceExcludingTax,
                unitPriceIncludingTax,
                vatRate,
                saleMode);

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
    }
}
