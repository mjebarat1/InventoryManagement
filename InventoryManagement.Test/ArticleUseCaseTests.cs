using InventoryManagement.Application.Articles.CreateNonFoodArticle;
using InventoryManagement.Application.Articles.GetArticleById;
using InventoryManagement.Application.Articles.SearchArticles;
using InventoryManagement.Application.Ports.Out;
using InventoryManagement.Domain.Articles;
using InventoryManagement.Domain.Shared.Exceptions;
using InventoryManagement.Domain.Shared.ValueObjects;

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
            new SearchArticlesQuery(1, 101, ArticleSortField.Reference, SortDirection.Asc, null, null, null)));
    }

    [Fact]
    public async Task GetArticleById_ReturnsNullWhenArticleDoesNotExist()
    {
        var useCase = new GetArticleByIdUseCase(new FakeArticleRepository());

        var result = await useCase.ExecuteAsync(new GetArticleByIdQuery(Guid.NewGuid()));

        Assert.Null(result);
    }

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

        public Task<ArticleSearchPage> SearchAsync(
            ArticleSearchCriteria criteria,
            CancellationToken cancellationToken = default)
            => Task.FromResult(new ArticleSearchPage(Array.Empty<Article>(), 0));
    }
}
