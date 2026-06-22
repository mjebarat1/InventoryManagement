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
    public sealed class NonFoodSupplyMovement : StockMovement
    {

        public PackagingLevel PackagingLevel { get; private set; }

        private NonFoodSupplyMovement()
        {
            // EF Core
        }

        private NonFoodSupplyMovement(Guid articleId, Quantity quantity, PackagingLevel packagingLevel)
            : base(articleId, quantity)
        {
            PackagingLevel = packagingLevel;
        }

        public static NonFoodSupplyMovement Create(
            Guid articleId,
            int quantity,
            PackagingLevel packagingLevel)
        {
            return new NonFoodSupplyMovement(
                articleId,
                Quantity.CreatePositive(quantity),
                packagingLevel
                );
        }


        public override Quantity ApplyTo(Quantity currentStock)
        {
            return currentStock + Quantity;
        }
    }
}
