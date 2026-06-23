using InventoryManagement.Application.Ports.In;
using InventoryManagement.Application.Ports.Out;
using InventoryManagement.Domain.Articles;
using InventoryManagement.Domain.Shared;
using InventoryManagement.Domain.Shared.Exceptions;
using InventoryManagement.Domain.Shared.ValueObjects;
using InventoryManagement.Domain.StockBucket;
using InventoryManagement.Domain.StockMovement;

namespace InventoryManagement.Application.Articles.RecordInventory;

public sealed class RecordInventoryUseCase : IRecordInventoryUseCase
{
    private readonly IArticleStockReadRepository _articleStockReadRepository;
    private readonly IStockBucketRepository _stockBucketRepository;
    private readonly IStockMovementRepository _stockMovementRepository;

    public RecordInventoryUseCase(
        IArticleStockReadRepository articleStockReadRepository,
        IStockBucketRepository stockBucketRepository,
        IStockMovementRepository stockMovementRepository)
    {
        _articleStockReadRepository = articleStockReadRepository;
        _stockBucketRepository = stockBucketRepository;
        _stockMovementRepository = stockMovementRepository;
    }

    public async Task<RecordInventoryResult?> ExecuteAsync(
        RecordInventoryCommand command,
        CancellationToken cancellationToken = default)
    {
        var snapshot = await _articleStockReadRepository.GetByArticleIdAsync(command.ArticleId, cancellationToken);
        if (snapshot is null)
            return null;

        snapshot.Article.EnsureActive();

        var existingCommands = command.ExistingBuckets
            ?? throw new BusinessRuleException(DomainErrorCodes.InventoryExistingBucketsRequired);
        var newCommands = command.NewBuckets
            ?? throw new BusinessRuleException(DomainErrorCodes.InventoryNewBucketsRequired);
        if (existingCommands.Count == 0 && newCommands.Count == 0)
            throw new BusinessRuleException(DomainErrorCodes.InventorySelectionRequired);

        var duplicateExistingBucket = existingCommands
            .GroupBy(item => item.StockBucketId)
            .FirstOrDefault(group => group.Count() > 1);
        if (duplicateExistingBucket is not null)
            throw new BusinessRuleException(DomainErrorCodes.InventoryDuplicateExistingBucket);

        var bucketsById = snapshot.Buckets.ToDictionary(bucket => bucket.Id);
        var quantitiesByBucket = snapshot.Movements
            .SelectMany(movement => movement.Lines)
            .GroupBy(line => line.StockBucketId)
            .ToDictionary(group => group.Key, group => group.Sum(line => line.QuantityDelta));
        var adjustments = new List<StockInventoryAdjustment>();

        foreach (var item in existingCommands)
        {
            if (!bucketsById.TryGetValue(item.StockBucketId, out var bucket))
                throw new BusinessRuleException(DomainErrorCodes.InventoryBucketNotInArticle);

            var currentQuantity = Quantity.Create(quantitiesByBucket.GetValueOrDefault(bucket.Id));
            var countedQuantity = Quantity.Create(item.CountedQuantity);
            if (!currentQuantity.Equals(countedQuantity))
                adjustments.Add(new StockInventoryAdjustment(bucket.Id, currentQuantity, countedQuantity));
        }

        var newReferences = new HashSet<string>(StringComparer.Ordinal);
        var newBuckets = new List<StockBucket>();
        foreach (var item in newCommands)
        {
            var reference = StockBucketReference.Create(item.Reference);
            if (!newReferences.Add(reference.Value))
                throw new BusinessRuleException(DomainErrorCodes.InventoryDuplicateNewReference);
            if (await _stockBucketRepository.ExistsByReferenceAsync(reference, cancellationToken))
            {
                throw new BusinessRuleException(
                    DomainErrorCodes.StockBucketReferenceAlreadyExists,
                    new Dictionary<string, object?> { ["reference"] = reference.Value });
            }

            var countedQuantity = Quantity.CreatePositive(item.CountedQuantity);
            var bucket = CreateNewBucket(snapshot.Article, reference, item);
            newBuckets.Add(bucket);
            adjustments.Add(new StockInventoryAdjustment(bucket.Id, Quantity.Create(0), countedQuantity));
        }

        var movement = InventoryMovement.Create(snapshot.Article.Id, command.Comment, adjustments);
        await _stockMovementRepository.AddInventoryAsync(newBuckets, movement, cancellationToken);

        return new RecordInventoryResult(
            movement.Id,
            movement.Lines.Count - newBuckets.Count,
            newBuckets.Count);
    }

    private static StockBucket CreateNewBucket(
        Article article,
        StockBucketReference reference,
        RecordInventoryNewBucketCommand command) => article switch
    {
        FoodArticle => CreateFoodBucket(article.Id, reference, command),
        NonFoodArticle => CreateNonFoodBucket(article.Id, reference, command),
        _ => throw new BusinessRuleException(DomainErrorCodes.ArticleTypeUnknown)
    };

    private static FoodStockBucket CreateFoodBucket(
        Guid articleId,
        StockBucketReference reference,
        RecordInventoryNewBucketCommand command)
    {
        if (command.ExpirationDate is null)
            throw new BusinessRuleException(DomainErrorCodes.StockBucketExpirationRequiredForFood);
        if (command.PackagingLevel is not null)
            throw new BusinessRuleException(DomainErrorCodes.StockBucketPackagingForbiddenForFood);

        return FoodStockBucket.Create(articleId, reference, command.ExpirationDate.Value);
    }

    private static NonFoodStockBucket CreateNonFoodBucket(
        Guid articleId,
        StockBucketReference reference,
        RecordInventoryNewBucketCommand command)
    {
        if (command.PackagingLevel is null)
            throw new BusinessRuleException(DomainErrorCodes.StockBucketPackagingRequiredForNonFood);
        if (command.ExpirationDate is not null)
            throw new BusinessRuleException(DomainErrorCodes.StockBucketExpirationForbiddenForNonFood);

        return NonFoodStockBucket.Create(articleId, reference, command.PackagingLevel.Value);
    }
}
