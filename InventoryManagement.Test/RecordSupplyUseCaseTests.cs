using InventoryManagement.Application.Articles.RecordSupply;
using InventoryManagement.Application.Ports.Out;
using InventoryManagement.Domain.Articles;
using InventoryManagement.Domain.Shared.Exceptions;
using InventoryManagement.Domain.Shared.ValueObjects;
using InventoryManagement.Domain.StockBucket;
using InventoryManagement.Domain.StockMovement;

namespace InventoryManagement.Test;

public sealed class RecordSupplyUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_CreatesFoodBucketAndPositiveMovementLine()
    {
        var article = CreateFoodArticle();
        var stockRepository = new FakeStockMovementRepository();
        var useCase = CreateUseCase(article, stockRepository);
        var expirationDate = new DateOnly(2026, 7, 15);

        var result = await useCase.ExecuteAsync(
            new RecordSupplyCommand(article.Id, "ref-lot-0000000000001", 8, expirationDate, null));

        Assert.NotNull(result);
        var bucket = Assert.IsType<FoodStockBucket>(stockRepository.Bucket);
        Assert.Equal(expirationDate, bucket.ExpirationDate);
        Assert.Equal("ref-lot-0000000000001", bucket.Reference.Value);
        var line = Assert.Single(stockRepository.Movement!.Lines);
        Assert.Equal(8, line.QuantityDelta);
        Assert.Equal(0, line.QuantityBefore!.Value);
        Assert.Equal(8, line.QuantityAfter!.Value);
    }

    [Fact]
    public async Task ExecuteAsync_CreatesNonFoodBucketWithPackaging()
    {
        var article = CreateNonFoodArticle();
        var stockRepository = new FakeStockMovementRepository();
        var useCase = CreateUseCase(article, stockRepository);

        await useCase.ExecuteAsync(
            new RecordSupplyCommand(article.Id, "ref-lot-0000000000002", 3, null, PackagingLevel.Refurbished));

        var bucket = Assert.IsType<NonFoodStockBucket>(stockRepository.Bucket);
        Assert.Equal(PackagingLevel.Refurbished, bucket.PackagingLevel);
        Assert.Equal(3, Assert.Single(stockRepository.Movement!.Lines).QuantityDelta);
    }

    [Fact]
    public async Task ExecuteAsync_RejectsNonPositiveQuantity()
    {
        var article = CreateFoodArticle();
        var useCase = CreateUseCase(article, new FakeStockMovementRepository());

        await Assert.ThrowsAsync<BusinessRuleException>(() => useCase.ExecuteAsync(
            new RecordSupplyCommand(article.Id, "ref-lot-0000000000003", 0, new DateOnly(2026, 7, 15), null)));
    }

    [Fact]
    public async Task ExecuteAsync_RejectsMissingTypeSpecificData()
    {
        var foodUseCase = CreateUseCase(CreateFoodArticle(), new FakeStockMovementRepository());
        var nonFoodUseCase = CreateUseCase(CreateNonFoodArticle(), new FakeStockMovementRepository());

        await Assert.ThrowsAsync<BusinessRuleException>(() => foodUseCase.ExecuteAsync(
            new RecordSupplyCommand(Guid.NewGuid(), "ref-lot-0000000000004", 2, null, null)));
        await Assert.ThrowsAsync<BusinessRuleException>(() => nonFoodUseCase.ExecuteAsync(
            new RecordSupplyCommand(Guid.NewGuid(), "ref-lot-0000000000005", 2, null, null)));
    }

    [Fact]
    public async Task ExecuteAsync_RejectsDataForAnotherArticleType()
    {
        var foodArticle = CreateFoodArticle();
        var nonFoodArticle = CreateNonFoodArticle();
        var foodUseCase = CreateUseCase(foodArticle, new FakeStockMovementRepository());
        var nonFoodUseCase = CreateUseCase(nonFoodArticle, new FakeStockMovementRepository());

        await Assert.ThrowsAsync<BusinessRuleException>(() => foodUseCase.ExecuteAsync(
            new RecordSupplyCommand(foodArticle.Id, "ref-lot-0000000000006", 2, new DateOnly(2026, 7, 15), PackagingLevel.New)));
        await Assert.ThrowsAsync<BusinessRuleException>(() => nonFoodUseCase.ExecuteAsync(
            new RecordSupplyCommand(nonFoodArticle.Id, "ref-lot-0000000000007", 2, new DateOnly(2026, 7, 15), PackagingLevel.New)));
    }

    [Fact]
    public async Task ExecuteAsync_ReturnsNullWhenArticleDoesNotExist()
    {
        var stockRepository = new FakeStockMovementRepository();
        var useCase = CreateUseCase(null, stockRepository);

        var result = await useCase.ExecuteAsync(
            new RecordSupplyCommand(Guid.NewGuid(), "ref-lot-0000000000008", 2, null, PackagingLevel.New));

        Assert.Null(result);
        Assert.Null(stockRepository.Movement);
    }

    [Fact]
    public async Task ExecuteAsync_RejectsInactiveArticle()
    {
        var article = CreateFoodArticle();
        article.Deactivate();
        var useCase = CreateUseCase(article, new FakeStockMovementRepository());

        await Assert.ThrowsAsync<BusinessRuleException>(() => useCase.ExecuteAsync(
            new RecordSupplyCommand(
                article.Id,
                "ref-lot-0000000000009",
                2,
                new DateOnly(2026, 7, 15),
                null)));
    }

    private static FoodArticle CreateFoodArticle() => FoodArticle.Create(
        Ean13Reference.Create("1234567890123"),
        "Yaourt",
        Money.FromDecimal(2.50m),
        new[] { SaleMode.TakeAway });

    private static NonFoodArticle CreateNonFoodArticle() => NonFoodArticle.Create(
        Ean13Reference.Create("9876543210123"),
        "Casque",
        Money.FromDecimal(99.99m));

    private static RecordSupplyUseCase CreateUseCase(Article? article, FakeStockMovementRepository repository) => new(
        new FakeArticleRepository(article),
        repository,
        new FakeStockBucketRepository());

    private sealed class FakeArticleRepository : IArticleRepository
    {
        private readonly Article? _article;

        public FakeArticleRepository(Article? article)
        {
            _article = article;
        }

        public Task AddAsync(Article article, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task<bool> ExistsByReferenceAsync(Ean13Reference reference, CancellationToken cancellationToken = default) => Task.FromResult(false);
        public Task<Article?> GetByIdAsync(Guid articleId, CancellationToken cancellationToken = default) => Task.FromResult(_article);
        public Task<Article?> GetForUpdateByIdAsync(Guid articleId, CancellationToken cancellationToken = default) => Task.FromResult(_article);
        public Task UpdateAsync(Article article, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task<ArticleSearchPage> SearchAsync(ArticleSearchCriteria criteria, CancellationToken cancellationToken = default)
            => Task.FromResult(new ArticleSearchPage(Array.Empty<Article>(), 0));
    }

    private sealed class FakeStockMovementRepository : IStockMovementRepository
    {
        public StockBucket? Bucket { get; private set; }
        public SupplyMovement? Movement { get; private set; }

        public Task AddSupplyAsync(
            StockBucket bucket,
            SupplyMovement movement,
            CancellationToken cancellationToken = default)
        {
            Bucket = bucket;
            Movement = movement;
            return Task.CompletedTask;
        }

        public Task AddSaleAsync(SaleMovement movement, CancellationToken cancellationToken = default)
            => Task.CompletedTask;

        public Task AddInventoryAsync(
            IReadOnlyCollection<StockBucket> newBuckets,
            InventoryMovement movement,
            CancellationToken cancellationToken = default) => Task.CompletedTask;
    }

    private sealed class FakeStockBucketRepository : IStockBucketRepository
    {
        public Task<bool> ExistsByReferenceAsync(
            StockBucketReference reference,
            CancellationToken cancellationToken = default) => Task.FromResult(false);

        public Task<IReadOnlyCollection<StockBucketQuantitySnapshot>> SearchByReferencePrefixAsync(
            Guid articleId,
            string referencePrefix,
            int maximumResults,
            CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyCollection<StockBucketQuantitySnapshot>>([]);
    }
}
