using InventoryManagement.Application.Articles.CreateNonFoodArticle;
using InventoryManagement.Application.Articles.GetArticleById;
using InventoryManagement.Application.Articles.SearchArticles;
using InventoryManagement.Application.Ports.Out;
using InventoryManagement.Domain.Articles;
using InventoryManagement.Domain.Shared.Exceptions;
using InventoryManagement.Domain.Shared.ValueObjects;
using InventoryManagement.Domain.StockBucket;
using InventoryManagement.Domain.StockMovement;

namespace InventoryManagement.Test;

public sealed class ArticleUseCaseTests
{
    [Fact]
    public async Task CreateNonFoodArticle_CreatesArticleWithoutPackaging()
    {
        var repository = new FakeArticleRepository();
        var useCase = new CreateNonFoodArticleUseCase(repository);

        var result = await useCase.ExecuteAsync(
            new CreateNonFoodArticleCommand("1234567890123", "Casque audio", 99.99m));

        Assert.NotEqual(Guid.Empty, result.ArticleId);
        var article = Assert.IsType<NonFoodArticle>(repository.AddedArticle);
        Assert.Equal("1234567890123", article.Reference.Value);
        Assert.Equal(99.99m, article.PriceExcludingTax.Amount);
    }

    [Fact]
    public async Task CreateNonFoodArticle_RejectsDuplicateReference()
    {
        var repository = new FakeArticleRepository { ReferenceExists = true };
        var useCase = new CreateNonFoodArticleUseCase(repository);

        await Assert.ThrowsAsync<BusinessRuleException>(() => useCase.ExecuteAsync(
            new CreateNonFoodArticleCommand("1234567890123", "Casque audio", 99.99m)));
    }

    [Fact]
    public async Task SearchArticles_RejectsPageSizeAboveOneHundred()
    {
        var useCase = new SearchArticlesUseCase(new FakeArticleRepository());

        await Assert.ThrowsAsync<BusinessRuleException>(() => useCase.ExecuteAsync(
            new SearchArticlesQuery(1, 101, ArticleSortField.Reference, SortDirection.Asc, null, null, ArticleActivityFilter.Active)));
    }

    [Fact]
    public async Task GetArticleById_ReturnsNullWhenArticleDoesNotExist()
    {
        var useCase = new GetArticleByIdUseCase(
            new FakeArticleStockReadRepository(null),
            new FakeClock(new DateTime(2026, 6, 23)));

        var result = await useCase.ExecuteAsync(new GetArticleByIdQuery(Guid.NewGuid()));

        Assert.Null(result);
    }

    [Fact]
    public async Task GetArticleById_CalculatesStocksFromMovementLineDeltas()
    {
        var article = NonFoodArticle.Create(
            Ean13Reference.Create("1234567890123"),
            "Casque audio",
            Money.FromDecimal(99.99m));
        var sellableBucket = NonFoodStockBucket.Create(article.Id, Reference(1), PackagingLevel.New);
        var unsellableBucket = NonFoodStockBucket.Create(article.Id, Reference(2), PackagingLevel.Unsellable);
        var movements = new StockMovement[]
        {
            SupplyMovement.Create(article.Id, sellableBucket.Id, Quantity.CreatePositive(10)),
            SupplyMovement.Create(article.Id, unsellableBucket.Id, Quantity.CreatePositive(3))
        };
        var snapshot = new ArticleStockSnapshot(
            article,
            new StockBucket[] { sellableBucket, unsellableBucket },
            movements);
        var useCase = new GetArticleByIdUseCase(
            new FakeArticleStockReadRepository(snapshot),
            new FakeClock(new DateTime(2026, 6, 23)));

        var result = await useCase.ExecuteAsync(new GetArticleByIdQuery(article.Id));

        Assert.NotNull(result);
        Assert.Equal(13, result.TotalStock);
        Assert.Equal(10, result.SellableStock);
        Assert.Equal(3, result.NonSellableStock);
        Assert.Contains(result.Buckets, bucket => bucket.Status == InventoryManagement.Application.Articles.Shared.StockBucketStatus.Sellable);
        Assert.Contains(result.Buckets, bucket => bucket.Status == InventoryManagement.Application.Articles.Shared.StockBucketStatus.Unsellable);
        Assert.All(result.Movements, movement => Assert.Equal(movement.QuantityDelta, movement.Lines.Sum(line => line.QuantityDelta)));
    }

    [Fact]
    public async Task GetArticleById_UsesClockForFoodBucketStatus()
    {
        var article = FoodArticle.Create(
            Ean13Reference.Create("1234567890123"),
            "Yaourt",
            Money.FromDecimal(2.50m),
            new[] { SaleMode.TakeAway });
        var expiredBucket = FoodStockBucket.Create(article.Id, Reference(3), new DateOnly(2026, 6, 22));
        var emptyBucket = FoodStockBucket.Create(article.Id, Reference(4), new DateOnly(2026, 7, 1));
        var snapshot = new ArticleStockSnapshot(
            article,
            new StockBucket[] { expiredBucket, emptyBucket },
            new StockMovement[]
            {
                SupplyMovement.Create(article.Id, expiredBucket.Id, Quantity.CreatePositive(5))
            });
        var useCase = new GetArticleByIdUseCase(
            new FakeArticleStockReadRepository(snapshot),
            new FakeClock(new DateTime(2026, 6, 23)));

        var result = await useCase.ExecuteAsync(new GetArticleByIdQuery(article.Id));

        Assert.NotNull(result);
        Assert.Equal(0, result.SellableStock);
        Assert.Equal(5, result.NonSellableStock);
        Assert.Contains(result.Buckets, bucket => bucket.Status == InventoryManagement.Application.Articles.Shared.StockBucketStatus.Expired);
        Assert.Contains(result.Buckets, bucket => bucket.Status == InventoryManagement.Application.Articles.Shared.StockBucketStatus.Empty);
    }

    private static StockBucketReference Reference(int value) =>
        StockBucketReference.Create($"ref-lot-{value:0000000000000}");

    private sealed class FakeArticleRepository : IArticleRepository
    {
        public bool ReferenceExists { get; init; }
        public Article? AddedArticle { get; private set; }
        public Article? ArticleToReturn { get; init; }

        public Task AddAsync(Article article, CancellationToken cancellationToken = default)
        {
            AddedArticle = article;
            return Task.CompletedTask;
        }

        public Task<bool> ExistsByReferenceAsync(Ean13Reference reference, CancellationToken cancellationToken = default)
            => Task.FromResult(ReferenceExists);

        public Task<Article?> GetByIdAsync(Guid articleId, CancellationToken cancellationToken = default)
            => Task.FromResult(ArticleToReturn);

        public Task<Article?> GetForUpdateByIdAsync(Guid articleId, CancellationToken cancellationToken = default)
            => Task.FromResult(ArticleToReturn);

        public Task UpdateAsync(Article article, CancellationToken cancellationToken = default)
            => Task.CompletedTask;

        public Task<ArticleSearchPage> SearchAsync(
            ArticleSearchCriteria criteria,
            CancellationToken cancellationToken = default)
            => Task.FromResult(new ArticleSearchPage(Array.Empty<Article>(), 0));
    }

    private sealed class FakeArticleStockReadRepository : IArticleStockReadRepository
    {
        private readonly ArticleStockSnapshot? _snapshot;

        public FakeArticleStockReadRepository(ArticleStockSnapshot? snapshot)
        {
            _snapshot = snapshot;
        }

        public Task<ArticleStockSnapshot?> GetByArticleIdAsync(
            Guid articleId,
            CancellationToken cancellationToken = default) => Task.FromResult(_snapshot);
    }

    private sealed class FakeClock : IClock
    {
        public FakeClock(DateTime today)
        {
            Today = today;
        }

        public DateTime Today { get; }
        public DateTime UtcNow => Today.ToUniversalTime();
    }
}
