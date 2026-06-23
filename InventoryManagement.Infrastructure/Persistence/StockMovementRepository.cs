using InventoryManagement.Application.Ports.Out;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagement.Infrastructure.Persistence
{
    internal class StockMovementRepository : IStockMovementRepository
    {
        private readonly StockDbContext _context;

        public StockMovementRepository(StockDbContext context)
        {
            _context = context;
        }

        public async Task AddSupplyAsync(
            Domain.StockBucket.StockBucket bucket,
            Domain.StockMovement.SupplyMovement movement,
            CancellationToken cancellationToken = default)
        {
            await _context.StockBuckets.AddAsync(bucket, cancellationToken);
            await _context.StockMovements.AddAsync(movement, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task AddSaleAsync(
            Domain.StockMovement.SaleMovement movement,
            CancellationToken cancellationToken = default)
        {
            await _context.StockMovements.AddAsync(movement, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task AddInventoryAsync(
            IReadOnlyCollection<Domain.StockBucket.StockBucket> newBuckets,
            Domain.StockMovement.InventoryMovement movement,
            CancellationToken cancellationToken = default)
        {
            await _context.StockBuckets.AddRangeAsync(newBuckets, cancellationToken);
            await _context.StockMovements.AddAsync(movement, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
