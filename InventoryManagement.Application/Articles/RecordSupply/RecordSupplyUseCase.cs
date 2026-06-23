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
    private readonly IStockBucketRepository _stockBucketRepository;

    public RecordSupplyUseCase(
        IArticleRepository articleRepository,
        IStockMovementRepository stockMovementRepository,
        IStockBucketRepository stockBucketRepository)
    {
        _articleRepository = articleRepository;
        _stockMovementRepository = stockMovementRepository;
        _stockBucketRepository = stockBucketRepository;
    }

    public async Task<RecordSupplyResult?> ExecuteAsync(
        RecordSupplyCommand command,
        CancellationToken cancellationToken = default)
    {
        var article = await _articleRepository.GetByIdAsync(command.ArticleId, cancellationToken);
        if (article is null)
            return null;

        article.EnsureActive();

        var reference = StockBucketReference.Create(command.StockBucketReference);
        if (await _stockBucketRepository.ExistsByReferenceAsync(reference, cancellationToken))
            throw new BusinessRuleException("Cette référence de lot existe déjà.");

        var quantity = Quantity.CreatePositive(command.Quantity);
        StockBucket bucket = article switch
        {
            FoodArticle => CreateFoodBucket(article.Id, reference, command),
            NonFoodArticle => CreateNonFoodBucket(article.Id, reference, command),
            _ => throw new BusinessRuleException("Type d'article inconnu.")
        };

        var movement = SupplyMovement.Create(article.Id, bucket.Id, quantity);
        await _stockMovementRepository.AddSupplyAsync(bucket, movement, cancellationToken);

        return new RecordSupplyResult(movement.Id, bucket.Id);
    }

    private static FoodStockBucket CreateFoodBucket(
        Guid articleId,
        StockBucketReference reference,
        RecordSupplyCommand command)
    {
        if (command.ExpirationDate is null)
            throw new BusinessRuleException("La DLC est obligatoire pour un approvisionnement alimentaire.");
        if (command.PackagingLevel is not null)
            throw new BusinessRuleException("Le packaging ne doit pas être renseigné pour un article alimentaire.");

        return FoodStockBucket.Create(articleId, reference, command.ExpirationDate.Value);
    }

    private static NonFoodStockBucket CreateNonFoodBucket(
        Guid articleId,
        StockBucketReference reference,
        RecordSupplyCommand command)
    {
        if (command.PackagingLevel is null)
            throw new BusinessRuleException("Le packaging est obligatoire pour un approvisionnement non alimentaire.");
        if (command.ExpirationDate is not null)
            throw new BusinessRuleException("La DLC ne doit pas être renseignée pour un article non alimentaire.");

        return NonFoodStockBucket.Create(articleId, reference, command.PackagingLevel.Value);
    }
}
