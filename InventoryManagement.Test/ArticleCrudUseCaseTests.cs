using InventoryManagement.Application.Articles.DeactivateArticle;
using InventoryManagement.Application.Articles.SearchArticles;
using InventoryManagement.Application.Articles.UpdateArticle;
using InventoryManagement.Application.Ports.Out;
using InventoryManagement.Domain.Articles;
using InventoryManagement.Domain.Shared.Exceptions;
using InventoryManagement.Domain.Shared.ValueObjects;

namespace InventoryManagement.Test;

public sealed class ArticleCrudUseCaseTests
{
    [Fact]
    public async Task UpdateArticle_UpdatesFoodFieldsAndSaleModesWithoutChangingReference()
    {
        var article = CreateFoodArticle();
        var originalReference = article.Reference;
        var repository = new FakeArticleRepository { Article = article };
        var useCase = new UpdateArticleUseCase(repository);

        var updated = await useCase.ExecuteAsync(new UpdateArticleCommand(
            article.Id,
            "Yaourt grec",
            3.25m,
            [SaleMode.OnSite]));

        Assert.True(updated);
        Assert.Equal("Yaourt grec", article.Name);
        Assert.Equal(3.25m, article.PriceExcludingTax.Amount);
        Assert.Equal(originalReference, article.Reference);
        Assert.Equal([SaleMode.OnSite], article.SaleModes);
        Assert.True(repository.UpdateCalled);
    }

    [Fact]
    public async Task UpdateArticle_RejectsSaleModesForNonFoodArticle()
    {
        var article = NonFoodArticle.Create(
            Ean13Reference.Create("9876543210123"),
            "Casque",
            Money.FromDecimal(100m));
        var useCase = new UpdateArticleUseCase(new FakeArticleRepository { Article = article });

        await Assert.ThrowsAsync<BusinessRuleException>(() => useCase.ExecuteAsync(new UpdateArticleCommand(
            article.Id,
            "Casque audio",
            90m,
            [SaleMode.TakeAway])));
    }

    [Fact]
    public async Task DeactivateArticle_MarksArticleInactiveAndPreservesIt()
    {
        var article = CreateFoodArticle();
        var repository = new FakeArticleRepository { Article = article };
        var useCase = new DeactivateArticleUseCase(repository);

        var deactivated = await useCase.ExecuteAsync(new DeactivateArticleCommand(article.Id));

        Assert.True(deactivated);
        Assert.False(article.IsActive);
        Assert.True(repository.UpdateCalled);
    }

    [Fact]
    public async Task DeactivateArticle_RejectsAlreadyInactiveArticle()
    {
        var article = CreateFoodArticle();
        article.Deactivate();
        var useCase = new DeactivateArticleUseCase(new FakeArticleRepository { Article = article });

        await Assert.ThrowsAsync<BusinessRuleException>(() =>
            useCase.ExecuteAsync(new DeactivateArticleCommand(article.Id)));
    }

    [Fact]
    public async Task SearchArticles_ForwardsCombinedTermAndActivityFilter()
    {
        var repository = new FakeArticleRepository();
        var useCase = new SearchArticlesUseCase(repository);

        await useCase.ExecuteAsync(new SearchArticlesQuery(
            1,
            20,
            ArticleSortField.Name,
            SortDirection.Asc,
            ArticleKind.Food,
            "yaourt",
            ArticleActivityFilter.Inactive));

        Assert.NotNull(repository.SearchCriteria);
        Assert.Equal("yaourt", repository.SearchCriteria.SearchTerm);
        Assert.Equal(ArticleActivityFilter.Inactive, repository.SearchCriteria.ActivityFilter);
        Assert.Equal(ArticleKind.Food, repository.SearchCriteria.Type);
    }

    private static FoodArticle CreateFoodArticle() => FoodArticle.Create(
        Ean13Reference.Create("1234567890123"),
        "Yaourt",
        Money.FromDecimal(2.50m),
        [SaleMode.TakeAway]);

    private sealed class FakeArticleRepository : IArticleRepository
    {
        public Article? Article { get; init; }
        public bool UpdateCalled { get; private set; }
        public ArticleSearchCriteria? SearchCriteria { get; private set; }

        public Task<bool> ExistsByReferenceAsync(Ean13Reference reference, CancellationToken cancellationToken = default)
            => Task.FromResult(false);

        public Task AddAsync(Article article, CancellationToken cancellationToken = default)
            => Task.CompletedTask;

        public Task<Article?> GetByIdAsync(Guid articleId, CancellationToken cancellationToken = default)
            => Task.FromResult(Article);

        public Task<Article?> GetForUpdateByIdAsync(Guid articleId, CancellationToken cancellationToken = default)
            => Task.FromResult(Article);

        public Task UpdateAsync(Article article, CancellationToken cancellationToken = default)
        {
            UpdateCalled = true;
            return Task.CompletedTask;
        }

        public Task<ArticleSearchPage> SearchAsync(
            ArticleSearchCriteria criteria,
            CancellationToken cancellationToken = default)
        {
            SearchCriteria = criteria;
            return Task.FromResult(new ArticleSearchPage([], 0));
        }
    }
}
