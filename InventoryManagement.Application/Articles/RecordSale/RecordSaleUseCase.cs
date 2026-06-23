using InventoryManagement.Application.Ports.In;
using InventoryManagement.Application.Ports.Out;
using InventoryManagement.Domain.Articles;
using InventoryManagement.Domain.Shared;
using InventoryManagement.Domain.Shared.Exceptions;
using InventoryManagement.Domain.Shared.ValueObjects;
using InventoryManagement.Domain.StockMovement;

namespace InventoryManagement.Application.Articles.RecordSale;

public sealed class RecordSaleUseCase : IRecordSaleUseCase
{
    private readonly IArticleStockReadRepository _articleStockReadRepository;
    private readonly IStockMovementRepository _stockMovementRepository;
    private readonly IClock _clock;
    private readonly StockSaleAllocator _allocator;

    public RecordSaleUseCase(
        IArticleStockReadRepository articleStockReadRepository,
        IStockMovementRepository stockMovementRepository,
        IClock clock,
        StockSaleAllocator allocator)
    {
        _articleStockReadRepository = articleStockReadRepository;
        _stockMovementRepository = stockMovementRepository;
        _clock = clock;
        _allocator = allocator;
    }

    public async Task<RecordSaleResult?> ExecuteAsync(
        RecordSaleCommand command,
        CancellationToken cancellationToken = default)
    {

        var snapshot = await _articleStockReadRepository.GetByArticleIdAsync(command.ArticleId, cancellationToken);
        if (snapshot is null)
            return null;

        snapshot.Article.EnsureActive();

        var quantity = Quantity.CreatePositive(command.Quantity);

        var (vatRate, priceIncludingTax, persistedSaleMode) = GetPricing(snapshot.Article, command.SaleMode);
        
        var quantitiesByBucket = snapshot.Movements
            .SelectMany(movement => movement.Lines)
            .GroupBy(line => line.StockBucketId)
            .ToDictionary(group => group.Key, group => group.Sum(line => line.QuantityDelta));
        
        var availabilities = snapshot.Buckets
            .Select(bucket => new StockBucketAvailability(
                bucket,
                Quantity.Create(Math.Max(0, quantitiesByBucket.GetValueOrDefault(bucket.Id)))))
            .ToArray();
        
        var consumptions = _allocator.Allocate(
            snapshot.Article,
            availabilities,
            quantity,
            DateOnly.FromDateTime(_clock.Today));
        
        var movement = SaleMovement.Create(
            snapshot.Article.Id,
            snapshot.Article.PriceExcludingTax,
            priceIncludingTax,
            vatRate,
            persistedSaleMode,
            consumptions);

        await _stockMovementRepository.AddSaleAsync(movement, cancellationToken);

        return new RecordSaleResult(
            movement.Id,
            Math.Abs(movement.Lines.Sum(line => line.QuantityDelta)));
    }

    private static (VatRate VatRate, Money PriceIncludingTax, SaleMode? SaleMode) GetPricing(
        Article article,
        SaleMode? saleMode)
    {
        if (article is FoodArticle)
        {
            if (saleMode is null)
                throw new BusinessRuleException("Le mode de vente est obligatoire pour un article alimentaire.");

            var foodVatRate = article.GetVatRate(saleMode.Value);
            return (foodVatRate, article.PriceExcludingTax.AddVat(foodVatRate), saleMode);
        }

        if (article is NonFoodArticle)
        {
            if (saleMode is not null)
                throw new BusinessRuleException("Le mode de vente ne doit pas être renseigné pour un article non alimentaire.");

            var nonFoodVatRate = VatRate.NonFood();
            return (nonFoodVatRate, article.PriceExcludingTax.AddVat(nonFoodVatRate), null);
        }

        throw new BusinessRuleException("Type d'article inconnu.");
    }
}
