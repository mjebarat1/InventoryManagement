using InventoryManagement.Domain.Articles;

namespace InventoryManagement.Api.DTO;

public sealed record RecordSaleRequest(int Quantity, SaleMode? SaleMode);
