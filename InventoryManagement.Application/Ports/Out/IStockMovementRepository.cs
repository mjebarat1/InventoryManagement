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

        Task AddInventoryAsync(
            IReadOnlyCollection<Domain.StockBucket.StockBucket> newBuckets,
            Domain.StockMovement.InventoryMovement movement,
            CancellationToken cancellationToken = default);
    }
}
