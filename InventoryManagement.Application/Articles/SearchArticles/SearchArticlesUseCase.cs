using InventoryManagement.Application.Articles.Shared;
using InventoryManagement.Application.Ports.In;
using InventoryManagement.Application.Ports.Out;
using InventoryManagement.Domain.Shared.Exceptions;

namespace InventoryManagement.Application.Articles.SearchArticles;

public sealed class SearchArticlesUseCase : ISearchArticlesUseCase
{
    private const int MaximumPageSize = 100;
    private readonly IArticleRepository _articleRepository;

    public SearchArticlesUseCase(IArticleRepository articleRepository) => _articleRepository = articleRepository;

    public async Task<PagedResult<ArticleSummaryResult>> ExecuteAsync(
        SearchArticlesQuery query,
        CancellationToken cancellationToken = default)
    {
        if (query.PageNumber < 1)
            throw new BusinessRuleException("Le numéro de page doit être supérieur ou égal à 1.");
        if (query.PageSize is < 1 or > MaximumPageSize)
            throw new BusinessRuleException($"La taille de page doit être comprise entre 1 et {MaximumPageSize}.");

        var page = await _articleRepository.SearchAsync(
            new ArticleSearchCriteria(query.PageNumber, query.PageSize, query.SortBy,
                query.SortDirection, query.Type, query.Reference, query.Name),
            cancellationToken);

        var totalPages = page.TotalItems == 0 ? 0 : (int)Math.Ceiling(page.TotalItems / (double)query.PageSize);

        return new PagedResult<ArticleSummaryResult>(
            page.Items.Select(ArticleReadModelMapper.ToSummary).ToArray(),
            query.PageNumber, query.PageSize, page.TotalItems, totalPages);
    }
}
