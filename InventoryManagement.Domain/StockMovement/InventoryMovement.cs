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

        public InventoryMovement(Guid articleId, Quantity countedQuantity)
            : base(articleId, countedQuantity)
        {
            // Ici Quantity = quantité réellement constatée.
        }

        public override Quantity ApplyTo(Quantity currentStock)
        {
            return Quantity;
        }
    }
}
