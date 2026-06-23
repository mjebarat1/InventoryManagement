using InventoryManagement.Application.Articles.RecordSale;
using InventoryManagement.Application.Ports.Out;
using InventoryManagement.Domain.Articles;
using InventoryManagement.Domain.Shared.Exceptions;
using InventoryManagement.Domain.Shared.ValueObjects;
using InventoryManagement.Domain.StockBucket;
using InventoryManagement.Domain.StockMovement;

namespace InventoryManagement.Test;

public sealed class RecordSaleUseCaseTests
{
    private static readonly DateTime Today = new(2026, 6, 23, 0, 0, 0, DateTimeKind.Utc);

    [Fact]
    public async Task ExecuteAsync_AllocatesFoodBucketsInFefoOrder()
    {
        var article = CreateFoodArticle(SaleMode.TakeAway, SaleMode.OnSite);
        var laterBucket = FoodStockBucket.Create(article.Id, Reference(1), new DateOnly(2026, 7, 10));
        var soonerBucket = FoodStockBucket.Create(article.Id, Reference(2), new DateOnly(2026, 6, 30));
        var snapshot = CreateSnapshot(article, (laterBucket, 10), (soonerBucket, 4));
        var repository = new FakeStockMovementRepository();
        var useCase = CreateUseCase(snapshot, repository);

        var result = await useCase.ExecuteAsync(new RecordSaleCommand(article.Id, 6, SaleMode.TakeAway));

        Assert.NotNull(result);
        Assert.Equal(6, result.SoldQuantity);
        var movement = Assert.IsType<SaleMovement>(repository.Sale);
        Assert.Equal(SaleMode.TakeAway, movement.SaleMode);
        Assert.Equal(0.055m, movement.VatRate.Value);
        Assert.Equal(2.64m, movement.UnitPriceIncludingTax.Amount);
        Assert.Collection(
            movement.Lines,
            first =>
            {
                Assert.Equal(soonerBucket.Id, first.StockBucketId);
                Assert.Equal(-4, first.QuantityDelta);
            },
            second =>
            {
                Assert.Equal(laterBucket.Id, second.StockBucketId);
                Assert.Equal(-2, second.QuantityDelta);
            });
    }

    [Fact]
    public async Task ExecuteAsync_RejectsInsufficientSellableStockWithoutPersistingSale()
    {
        var article = CreateFoodArticle(SaleMode.TakeAway);
        var sellableBucket = FoodStockBucket.Create(article.Id, Reference(3), new DateOnly(2026, 6, 30));
        var expiredBucket = FoodStockBucket.Create(article.Id, Reference(4), new DateOnly(2026, 6, 22));
        var repository = new FakeStockMovementRepository();
        var useCase = CreateUseCase(
            CreateSnapshot(article, (sellableBucket, 3), (expiredBucket, 20)),
            repository);

        var exception = await Assert.ThrowsAsync<BusinessRuleException>(() =>
            useCase.ExecuteAsync(new RecordSaleCommand(article.Id, 4, SaleMode.TakeAway)));

        Assert.Equal(DomainErrorCodes.StockInsufficient, exception.Code);
        Assert.Equal(4, exception.Parameters["requestedQuantity"]);
        Assert.Equal(3, exception.Parameters["availableQuantity"]);
        Assert.Null(repository.Sale);
    }

    [Fact]
    public async Task ExecuteAsync_RejectsUnauthorizedFoodSaleMode()
    {
        var article = CreateFoodArticle(SaleMode.TakeAway);
        var bucket = FoodStockBucket.Create(article.Id, Reference(5), new DateOnly(2026, 6, 30));
        var useCase = CreateUseCase(
            CreateSnapshot(article, (bucket, 3)),
            new FakeStockMovementRepository());

        await Assert.ThrowsAsync<BusinessRuleException>(() =>
            useCase.ExecuteAsync(new RecordSaleCommand(article.Id, 1, SaleMode.OnSite)));
    }

    [Fact]
    public async Task ExecuteAsync_NonFoodExcludesUnsellableBucketsAndStoresTwentyPercentVat()
    {
        var article = NonFoodArticle.Create(
            Ean13Reference.Create("9876543210123"),
            "Casque",
            Money.FromDecimal(100m));
        var unsellable = NonFoodStockBucket.Create(article.Id, Reference(6), PackagingLevel.Unsellable);
        var refurbished = NonFoodStockBucket.Create(article.Id, Reference(7), PackagingLevel.Refurbished);
        var repository = new FakeStockMovementRepository();
        var useCase = CreateUseCase(
            CreateSnapshot(article, (unsellable, 50), (refurbished, 2)),
            repository);

        await useCase.ExecuteAsync(new RecordSaleCommand(article.Id, 2, null));

        var movement = Assert.IsType<SaleMovement>(repository.Sale);
        var line = Assert.Single(movement.Lines);
        Assert.Equal(refurbished.Id, line.StockBucketId);
        Assert.Equal(-2, line.QuantityDelta);
        Assert.Null(movement.SaleMode);
        Assert.Equal(0.20m, movement.VatRate.Value);
        Assert.Equal(120m, movement.UnitPriceIncludingTax.Amount);
    }

    [Fact]
    public async Task ExecuteAsync_RejectsSaleModeForNonFoodArticle()
    {
        var article = NonFoodArticle.Create(
            Ean13Reference.Create("9876543210123"),
            "Casque",
            Money.FromDecimal(100m));

        var useCase = CreateUseCase(
            new ArticleStockSnapshot(article, Array.Empty<StockBucket>(), Array.Empty<StockMovement>()),
            new FakeStockMovementRepository());

        await Assert.ThrowsAsync<BusinessRuleException>(() =>
            useCase.ExecuteAsync(new RecordSaleCommand(article.Id, 1, SaleMode.TakeAway)));
    }

    private static FoodArticle CreateFoodArticle(params SaleMode[] saleModes) => FoodArticle.Create(
        Ean13Reference.Create("1234567890123"),
        "Yaourt",
        Money.FromDecimal(2.50m),
        saleModes);

    private static StockBucketReference Reference(int value) =>
        StockBucketReference.Create($"ref-lot-{value:0000000000000}");

    private static ArticleStockSnapshot CreateSnapshot(
        Article article,
        params (StockBucket Bucket, int Quantity)[] stocks)
    {
        var movements = stocks
            .Select(stock => (StockMovement)SupplyMovement.Create(
                article.Id,
                stock.Bucket.Id,
                Quantity.CreatePositive(stock.Quantity)))
            .ToArray();

        return new ArticleStockSnapshot(article, stocks.Select(stock => stock.Bucket).ToArray(), movements);
    }

    private static RecordSaleUseCase CreateUseCase(
        ArticleStockSnapshot snapshot,
        FakeStockMovementRepository repository) => new(
            new FakeArticleStockReadRepository(snapshot),
            repository,
            new FakeClock(),
            new StockSaleAllocator());

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

    private sealed class FakeStockMovementRepository : IStockMovementRepository
    {
        public SaleMovement? Sale { get; private set; }

        public Task AddSupplyAsync(
            StockBucket bucket,
            SupplyMovement movement,
            CancellationToken cancellationToken = default) => Task.CompletedTask;

        public Task AddSaleAsync(SaleMovement movement, CancellationToken cancellationToken = default)
        {
            Sale = movement;
            return Task.CompletedTask;
        }

        public Task AddInventoryAsync(
            IReadOnlyCollection<StockBucket> newBuckets,
            InventoryMovement movement,
            CancellationToken cancellationToken = default) => Task.CompletedTask;
    }

    private sealed class FakeClock : IClock
    {
        public DateTime Today => RecordSaleUseCaseTests.Today;
        public DateTime UtcNow => RecordSaleUseCaseTests.Today;
    }
}
