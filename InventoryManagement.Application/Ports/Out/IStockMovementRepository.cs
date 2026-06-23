using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagement.Application.Ports.Out
{
    public interface IStockMovementRepository
    {
        Task AddSupplyAsync(
            Domain.StockBucket.StockBucket bucket,
            Domain.StockMovement.SupplyMovement movement,
            CancellationToken cancellationToken = default);

        Task AddSaleAsync(
            Domain.StockMovement.SaleMovement movement,
            CancellationToken cancellationToken = default);
    }
}
