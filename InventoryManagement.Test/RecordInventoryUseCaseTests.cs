using InventoryManagement.Application.Articles.RecordInventory;
using InventoryManagement.Application.Ports.Out;
using InventoryManagement.Domain.Articles;
using InventoryManagement.Domain.Shared;
using InventoryManagement.Domain.Shared.Exceptions;
using InventoryManagement.Domain.Shared.ValueObjects;
using InventoryManagement.Domain.StockBucket;
using InventoryManagement.Domain.StockMovement;

namespace InventoryManagement.Test;

public sealed class RecordInventoryUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_RecalculatesCurrentQuantityAndCreatesExistingAndNewLines()
    {
        var article = CreateNonFoodArticle();
        var existingBucket = NonFoodStockBucket.Create(article.Id, Reference(1), PackagingLevel.New);
        var supply = SupplyMovement.Create(article.Id, existingBucket.Id, Quantity.CreatePositive(10));
        var previousInventory = InventoryMovement.Create(
            article.Id,
            null,
            [new StockInventoryAdjustment(existingBucket.Id, Quantity.Create(10), Quantity.Create(7))]);
        var snapshot = new ArticleStockSnapshot(
            article,
            [existingBucket],
            new StockMovement[] { supply, previousInventory });
        var movementRepository = new FakeStockMovementRepository();
        var useCase = CreateUseCase(snapshot, movementRepository);

        var result = await useCase.ExecuteAsync(new RecordInventoryCommand(
            article.Id,
            "Inventaire mensuel",
            [new RecordInventoryExistingBucketCommand(existingBucket.Id, 5)],
            [new RecordInventoryNewBucketCommand(
                Reference(2).Value,
                4,
                null,
                PackagingLevel.Refurbished)]));

        Assert.NotNull(result);
        Assert.Equal(1, result.AdjustedBucketCount);
        Assert.Equal(1, result.CreatedBucketCount);
        var movement = Assert.IsType<InventoryMovement>(movementRepository.Inventory);
        Assert.Equal("Inventaire mensuel", movement.Comment);
        Assert.Collection(
            movement.Lines,
            existingLine =>
            {
                Assert.Equal(existingBucket.Id, existingLine.StockBucketId);
                Assert.Equal(7, existingLine.QuantityBefore!.Value);
                Assert.Equal(-2, existingLine.QuantityDelta);
                Assert.Equal(5, existingLine.QuantityAfter!.Value);
            },
            newLine =>
            {
                Assert.Equal(0, newLine.QuantityBefore!.Value);
                Assert.Equal(4, newLine.QuantityDelta);
                Assert.Equal(4, newLine.QuantityAfter!.Value);
            });
        var createdBucket = Assert.IsType<NonFoodStockBucket>(Assert.Single(movementRepository.NewBuckets));
        Assert.Equal(Reference(2), createdBucket.Reference);
        Assert.Equal(PackagingLevel.Refurbished, createdBucket.PackagingLevel);
    }

    [Fact]
    public async Task ExecuteAsync_RejectsInventoryWithoutEffectiveDifference()
    {
        var article = CreateNonFoodArticle();
        var bucket = NonFoodStockBucket.Create(article.Id, Reference(1), PackagingLevel.New);
        var snapshot = new ArticleStockSnapshot(
            article,
            [bucket],
            [SupplyMovement.Create(article.Id, bucket.Id, Quantity.CreatePositive(3))]);
        var movementRepository = new FakeStockMovementRepository();
        var useCase = CreateUseCase(snapshot, movementRepository);

        await Assert.ThrowsAsync<BusinessRuleException>(() => useCase.ExecuteAsync(new RecordInventoryCommand(
            article.Id,
            null,
            [new RecordInventoryExistingBucketCommand(bucket.Id, 3)],
            [])));

        Assert.Null(movementRepository.Inventory);
    }

    [Fact]
    public async Task ExecuteAsync_RejectsUnknownOrDuplicatedExistingBucket()
    {
        var article = CreateNonFoodArticle();
        var snapshot = new ArticleStockSnapshot(article, [], []);
        var useCase = CreateUseCase(snapshot, new FakeStockMovementRepository());
        var unknownId = Guid.NewGuid();

        await Assert.ThrowsAsync<BusinessRuleException>(() => useCase.ExecuteAsync(new RecordInventoryCommand(
            article.Id,
            null,
            [new(unknownId, 0), new(unknownId, 1)],
            [])));
    }

    [Fact]
    public async Task ExecuteAsync_RejectsExistingGlobalReferenceForNewBucket()
    {
        var article = CreateNonFoodArticle();
        var referenceRepository = new FakeStockBucketRepository { ReferenceExists = true };
        var useCase = new RecordInventoryUseCase(
            new FakeArticleStockReadRepository(new ArticleStockSnapshot(article, [], [])),
            referenceRepository,
            new FakeStockMovementRepository());

        await Assert.ThrowsAsync<BusinessRuleException>(() => useCase.ExecuteAsync(new RecordInventoryCommand(
            article.Id,
            null,
            [],
            [new(Reference(3).Value, 1, null, PackagingLevel.New)])));
    }

    private static NonFoodArticle CreateNonFoodArticle() => NonFoodArticle.Create(
        Ean13Reference.Create("9876543210123"),
        "Casque",
        Money.FromDecimal(100m));

    private static StockBucketReference Reference(int value) =>
        StockBucketReference.Create($"ref-lot-{value:0000000000000}");

    private static RecordInventoryUseCase CreateUseCase(
        ArticleStockSnapshot snapshot,
        FakeStockMovementRepository movementRepository) => new(
            new FakeArticleStockReadRepository(snapshot),
            new FakeStockBucketRepository(),
            movementRepository);

    private sealed class FakeArticleStockReadRepository : IArticleStockReadRepository
    {
        private readonly ArticleStockSnapshot _snapshot;

        public FakeArticleStockReadRepository(ArticleStockSnapshot snapshot)
        {
            _snapshot = snapshot;
        }

        public Task<ArticleStockSnapshot?> GetByArticleIdAsync(
            Guid articleId,
            CancellationToken cancellationToken = default) => Task.FromResult<ArticleStockSnapshot?>(_snapshot);
    }

    private sealed class FakeStockBucketRepository : IStockBucketRepository
    {
        public bool ReferenceExists { get; init; }

        public Task<bool> ExistsByReferenceAsync(
            StockBucketReference reference,
            CancellationToken cancellationToken = default) => Task.FromResult(ReferenceExists);

        public Task<IReadOnlyCollection<StockBucketQuantitySnapshot>> SearchByReferencePrefixAsync(
            Guid articleId,
            string referencePrefix,
            int maximumResults,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyCollection<StockBucketQuantitySnapshot>>([]);
    }

    private sealed class FakeStockMovementRepository : IStockMovementRepository
    {
        public InventoryMovement? Inventory { get; private set; }
        public IReadOnlyCollection<StockBucket> NewBuckets { get; private set; } = [];

        public Task AddSupplyAsync(StockBucket bucket, SupplyMovement movement, CancellationToken cancellationToken = default)
            => Task.CompletedTask;

        public Task AddSaleAsync(SaleMovement movement, CancellationToken cancellationToken = default)
            => Task.CompletedTask;

        public Task AddInventoryAsync(
            IReadOnlyCollection<StockBucket> newBuckets,
            InventoryMovement movement,
            CancellationToken cancellationToken = default)
        {
            NewBuckets = newBuckets;
            Inventory = movement;
            return Task.CompletedTask;
        }
    }
}
