using InventoryManagement.Domain.Articles;

namespace InventoryManagement.Application.Articles.UpdateArticle;

public sealed record UpdateArticleCommand(
    Guid ArticleId,
    string Name,
    decimal PriceExcludingTax,
    IReadOnlyCollection<SaleMode>? AllowedSaleModes);
