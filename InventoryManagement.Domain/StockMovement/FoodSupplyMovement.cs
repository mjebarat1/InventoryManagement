using InventoryManagement.Domain.Shared.Exceptions;
using InventoryManagement.Domain.Shared.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagement.Domain.StockMovement
{
    public sealed class FoodSupplyMovement : StockMovement
    {

        public DateOnly ExpirationDate { get; private set; }


        private FoodSupplyMovement()
        {
            // EF Core
        }

        private FoodSupplyMovement(Guid articleId, Quantity quantity, DateOnly expirationDate)
            : base(articleId, quantity)
        {
            ExpirationDate = expirationDate;
        }

        public static FoodSupplyMovement Create(
            Guid articleId,
            int quantity,
            DateOnly expirationDate)
        {
            return new FoodSupplyMovement(
                articleId,
                Quantity.CreatePositive(quantity),
                expirationDate);
        }

        public override Quantity ApplyTo(Quantity currentStock)
        {
            return currentStock + Quantity;
        }
    }
}
