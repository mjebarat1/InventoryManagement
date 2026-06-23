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
        var useCase = new RecordSupplyUseCase(new FakeArticleRepository(article), stockRepository);
        var expirationDate = new DateOnly(2026, 7, 15);

        var result = await useCase.ExecuteAsync(
            new RecordSupplyCommand(article.Id, 8, expirationDate, null));

        Assert.NotNull(result);
        var bucket = Assert.IsType<FoodStockBucket>(stockRepository.Bucket);
        Assert.Equal(expirationDate, bucket.ExpirationDate);
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
        var useCase = new RecordSupplyUseCase(new FakeArticleRepository(article), stockRepository);

        await useCase.ExecuteAsync(
            new RecordSupplyCommand(article.Id, 3, null, PackagingLevel.Refurbished));

        var bucket = Assert.IsType<NonFoodStockBucket>(stockRepository.Bucket);
        Assert.Equal(PackagingLevel.Refurbished, bucket.PackagingLevel);
        Assert.Equal(3, Assert.Single(stockRepository.Movement!.Lines).QuantityDelta);
    }

    [Fact]
    public async Task ExecuteAsync_RejectsNonPositiveQuantity()
    {
        var article = CreateFoodArticle();
        var useCase = new RecordSupplyUseCase(
            new FakeArticleRepository(article),
            new FakeStockMovementRepository());

        await Assert.ThrowsAsync<BusinessRuleException>(() => useCase.ExecuteAsync(
            new RecordSupplyCommand(article.Id, 0, new DateOnly(2026, 7, 15), null)));
    }

    [Fact]
    public async Task ExecuteAsync_RejectsMissingTypeSpecificData()
    {
        var foodUseCase = new RecordSupplyUseCase(
            new FakeArticleRepository(CreateFoodArticle()),
            new FakeStockMovementRepository());
        var nonFoodUseCase = new RecordSupplyUseCase(
            new FakeArticleRepository(CreateNonFoodArticle()),
            new FakeStockMovementRepository());

        await Assert.ThrowsAsync<BusinessRuleException>(() => foodUseCase.ExecuteAsync(
            new RecordSupplyCommand(Guid.NewGuid(), 2, null, null)));
        await Assert.ThrowsAsync<BusinessRuleException>(() => nonFoodUseCase.ExecuteAsync(
            new RecordSupplyCommand(Guid.NewGuid(), 2, null, null)));
    }

    [Fact]
    public async Task ExecuteAsync_RejectsDataForAnotherArticleType()
    {
        var foodArticle = CreateFoodArticle();
        var nonFoodArticle = CreateNonFoodArticle();
        var foodUseCase = new RecordSupplyUseCase(
            new FakeArticleRepository(foodArticle),
            new FakeStockMovementRepository());
        var nonFoodUseCase = new RecordSupplyUseCase(
            new FakeArticleRepository(nonFoodArticle),
            new FakeStockMovementRepository());

        await Assert.ThrowsAsync<BusinessRuleException>(() => foodUseCase.ExecuteAsync(
            new RecordSupplyCommand(foodArticle.Id, 2, new DateOnly(2026, 7, 15), PackagingLevel.New)));
        await Assert.ThrowsAsync<BusinessRuleException>(() => nonFoodUseCase.ExecuteAsync(
            new RecordSupplyCommand(nonFoodArticle.Id, 2, new DateOnly(2026, 7, 15), PackagingLevel.New)));
    }

    [Fact]
    public async Task ExecuteAsync_ReturnsNullWhenArticleDoesNotExist()
    {
        var stockRepository = new FakeStockMovementRepository();
        var useCase = new RecordSupplyUseCase(new FakeArticleRepository(null), stockRepository);

        var result = await useCase.ExecuteAsync(
            new RecordSupplyCommand(Guid.NewGuid(), 2, null, PackagingLevel.New));

        Assert.Null(result);
        Assert.Null(stockRepository.Movement);
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
    }
}
