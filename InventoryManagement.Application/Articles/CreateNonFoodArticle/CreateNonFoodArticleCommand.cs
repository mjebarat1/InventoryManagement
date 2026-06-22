namespace InventoryManagement.Application.Articles.CreateNonFoodArticle;

public sealed record CreateNonFoodArticleCommand(
    string Reference,
    string Name,
    decimal PriceExcludingTax);
