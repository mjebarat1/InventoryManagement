using InventoryManagement.Application.Ports.In;
using InventoryManagement.Application.Ports.Out;
using InventoryManagement.Domain.Articles;
using InventoryManagement.Domain.Shared.Exceptions;
using InventoryManagement.Domain.Shared.ValueObjects;
using InventoryManagement.Domain.StockBucket;
using InventoryManagement.Domain.StockMovement;

namespace InventoryManagement.Application.Articles.RecordSupply;

public sealed class RecordSupplyUseCase : IRecordSupplyUseCase
{
    private readonly IArticleRepository _articleRepository;
    private readonly IStockMovementRepository _stockMovementRepository;

    public RecordSupplyUseCase(
        IArticleRepository articleRepository,
        IStockMovementRepository stockMovementRepository)
    {
        _articleRepository = articleRepository;
        _stockMovementRepository = stockMovementRepository;
    }

    public async Task<RecordSupplyResult?> ExecuteAsync(
        RecordSupplyCommand command,
        CancellationToken cancellationToken = default)
    {
        var article = await _articleRepository.GetByIdAsync(command.ArticleId, cancellationToken);
        if (article is null)
            return null;

        var quantity = Quantity.CreatePositive(command.Quantity);
        StockBucket bucket = article switch
        {
            FoodArticle => CreateFoodBucket(article.Id, command),
            NonFoodArticle => CreateNonFoodBucket(article.Id, command),
            _ => throw new BusinessRuleException("Type d'article inconnu.")
        };

        var movement = SupplyMovement.Create(article.Id, bucket.Id, quantity);
        await _stockMovementRepository.AddSupplyAsync(bucket, movement, cancellationToken);

        return new RecordSupplyResult(movement.Id, bucket.Id);
    }

    private static FoodStockBucket CreateFoodBucket(Guid articleId, RecordSupplyCommand command)
    {
        if (command.ExpirationDate is null)
            throw new BusinessRuleException("La DLC est obligatoire pour un approvisionnement alimentaire.");
        if (command.PackagingLevel is not null)
            throw new BusinessRuleException("Le packaging ne doit pas être renseigné pour un article alimentaire.");

        return FoodStockBucket.Create(articleId, command.ExpirationDate.Value);
    }

    private static NonFoodStockBucket CreateNonFoodBucket(Guid articleId, RecordSupplyCommand command)
    {
        if (command.PackagingLevel is null)
            throw new BusinessRuleException("Le packaging est obligatoire pour un approvisionnement non alimentaire.");
        if (command.ExpirationDate is not null)
            throw new BusinessRuleException("La DLC ne doit pas être renseignée pour un article non alimentaire.");

        return NonFoodStockBucket.Create(articleId, command.PackagingLevel.Value);
    }
}
