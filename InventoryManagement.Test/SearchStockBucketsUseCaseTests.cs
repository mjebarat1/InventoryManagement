using InventoryManagement.Application.Articles.SearchStockBuckets;
using InventoryManagement.Application.Ports.Out;
using InventoryManagement.Domain.Articles;
using InventoryManagement.Domain.Shared.Exceptions;
using InventoryManagement.Domain.Shared.ValueObjects;
using InventoryManagement.Domain.StockBucket;

namespace InventoryManagement.Test;

public sealed class SearchStockBucketsUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_SearchesCurrentArticleByPrefixedDigitsAndLimitsResults()
    {
        var articleId = Guid.NewGuid();
        var bucket = NonFoodStockBucket.Create(
            articleId,
            StockBucketReference.Create("ref-lot-0000000000042"),
            PackagingLevel.New);
        var repository = new FakeStockBucketRepository(
            [new StockBucketQuantitySnapshot(bucket, 7)]);
        var useCase = new SearchStockBucketsUseCase(repository, new FakeClock());

        var result = await useCase.ExecuteAsync(new SearchStockBucketsQuery(articleId, "000000000"));

        var item = Assert.Single(result);
        Assert.Equal("ref-lot-0000000000042", item.Reference);
        Assert.Equal(7, item.PhysicalQuantity);
        Assert.Equal(articleId, repository.ArticleId);
        Assert.Equal("ref-lot-000000000", repository.ReferencePrefix);
        Assert.Equal(20, repository.MaximumResults);
    }

    [Theory]
    [InlineData("12345678")]
    [InlineData("12345678901234")]
    [InlineData("12345678x")]
    public async Task ExecuteAsync_RejectsInvalidLazySearchDigits(string digits)
    {
        var useCase = new SearchStockBucketsUseCase(new FakeStockBucketRepository([]), new FakeClock());

        await Assert.ThrowsAsync<BusinessRuleException>(() =>
            useCase.ExecuteAsync(new SearchStockBucketsQuery(Guid.NewGuid(), digits)));
    }

    private sealed class FakeStockBucketRepository : IStockBucketRepository
    {
        private readonly IReadOnlyCollection<StockBucketQuantitySnapshot> _results;

        public FakeStockBucketRepository(IReadOnlyCollection<StockBucketQuantitySnapshot> results)
        {
            _results = results;
        }

        public Guid ArticleId { get; private set; }
        public string? ReferencePrefix { get; private set; }
        public int MaximumResults { get; private set; }

        public Task<bool> ExistsByReferenceAsync(
            StockBucketReference reference,
            CancellationToken cancellationToken = default) => Task.FromResult(false);

        public Task<IReadOnlyCollection<StockBucketQuantitySnapshot>> SearchByReferencePrefixAsync(
            Guid articleId,
            string referencePrefix,
            int maximumResults,
            CancellationToken cancellationToken = default)
        {
            ArticleId = articleId;
            ReferencePrefix = referencePrefix;
            MaximumResults = maximumResults;
            return Task.FromResult(_results);
        }
    }

    private sealed class FakeClock : IClock
    {
        public DateTime Today => new(2026, 6, 23);
        public DateTime UtcNow => Today;
    }
}
