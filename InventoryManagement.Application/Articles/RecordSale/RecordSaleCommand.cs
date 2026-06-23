using InventoryManagement.Domain.Articles;

namespace InventoryManagement.Application.Articles.RecordSale;

public sealed record RecordSaleCommand(Guid ArticleId, int Quantity, SaleMode? SaleMode);

public sealed record RecordSaleResult(Guid MovementId, int SoldQuantity);
