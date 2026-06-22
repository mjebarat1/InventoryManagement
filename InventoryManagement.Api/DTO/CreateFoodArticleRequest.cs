using InventoryManagement.Domain.Articles;

namespace InventoryManagement.Api.DTO
{
    public record CreateFoodArticleRequest(
        string Reference,
        string Name,
        decimal PriceExcludingTax,
        IReadOnlyCollection<SaleMode> SaleModes);
}
