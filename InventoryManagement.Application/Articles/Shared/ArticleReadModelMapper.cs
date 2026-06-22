using InventoryManagement.Application.Ports.Out;
using InventoryManagement.Domain.Articles;
using InventoryManagement.Domain.Shared.ValueObjects;
using InventoryManagement.Domain.StockMovement;

namespace InventoryManagement.Application.Articles.Shared;

internal static class ArticleReadModelMapper
{
    public static ArticleSummaryResult ToSummary(Article article) => new(
        article.Id,
        article.Reference.Value,
        article.Name,
        GetArticleKind(article),
        article.PriceExcludingTax.Amount,
        GetPrices(article),
        CalculateTotalStock(article.StockMovements));

    public static ArticleDetailsResult ToDetails(Article article) => new(
        article.Id,
        article.Reference.Value,
        article.Name,
        GetArticleKind(article),
        article.PriceExcludingTax.Amount,
        article is FoodArticle foodArticle ? foodArticle.SaleModes : Array.Empty<SaleMode>(),
        GetPrices(article),
        CalculateTotalStock(article.StockMovements),
        null,
        null,
        article.StockMovements.OrderByDescending(movement => movement.CreatedAt).Select(ToMovement).ToArray());

    private static ArticleKind GetArticleKind(Article article) => article switch
    {
        FoodArticle => ArticleKind.Food,
        NonFoodArticle => ArticleKind.NonFood,
        _ => throw new InvalidOperationException("Type d'article inconnu.")
    };

    private static IReadOnlyCollection<ArticlePriceResult> GetPrices(Article article)
    {
        if (article is FoodArticle foodArticle)
            return foodArticle.SaleModes.Select(mode => CreatePrice(article, mode, mode)).ToArray();

        return [CreatePrice(article, SaleMode.TakeAway, null)];
    }

    private static ArticlePriceResult CreatePrice(Article article, SaleMode calculationMode, SaleMode? displayedMode)
    {
        var vatRate = article.GetVatRate(calculationMode);
        return new ArticlePriceResult(
            displayedMode,
            vatRate.Value,
            article.GetPriceIncludingTax(calculationMode).Amount);
    }

    private static int CalculateTotalStock(IEnumerable<StockMovement> movements)
    {
        var stock = Quantity.Create(0);
        foreach (var movement in movements.OrderBy(item => item.CreatedAt))
            stock = movement.ApplyTo(stock);
        return stock.Value;
    }

    private static StockMovementResult ToMovement(StockMovement movement) => new(
        movement.Id,
        movement.CreatedAt,
        movement switch
        {
            FoodSupplyMovement => "FoodSupply",
            NonFoodSupplyMovement => "NonFoodSupply",
            SaleMovement => "Sale",
            InventoryMovement => "Inventory",
            _ => "Unknown"
        },
        movement.Quantity.Value,
        movement is FoodSupplyMovement foodSupply ? foodSupply.ExpirationDate : null,
        movement is NonFoodSupplyMovement nonFoodSupply ? nonFoodSupply.PackagingLevel : null,
        null);
}
