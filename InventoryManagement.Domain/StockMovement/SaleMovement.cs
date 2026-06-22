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

        public SaleMovement(Guid articleId, Quantity quantity, Money unitPriceExcludingTax, Money unitPriceIncludingTax, VatRate VatRate)
            : base(articleId, quantity)
        {
            UnitPriceExcludingTax = unitPriceExcludingTax;
            UnitPriceIncludingTax = unitPriceIncludingTax;
            this.VatRate = VatRate;
        }

        public override Quantity ApplyTo(Quantity currentStock)
        {
            return currentStock - Quantity;
        }
    }
}
