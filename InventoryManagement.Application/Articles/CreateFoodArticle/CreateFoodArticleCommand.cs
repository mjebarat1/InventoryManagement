using InventoryManagement.Domain.Articles;

namespace InventoryManagement.Application.Articles.CreateFoodArticle
{
    public sealed record CreateFoodArticleCommand(
        string Reference,
        string Name,
        decimal PriceExcludingTax,
        IEnumerable<SaleMode> SaleModes);
}
