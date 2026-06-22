using InventoryManagement.Application.Abstractions;
using InventoryManagement.Application.Articles.SearchArticles;
using InventoryManagement.Application.Articles.Shared;

namespace InventoryManagement.Application.Ports.In;

public interface ISearchArticlesUseCase
    : IUseCase<SearchArticlesQuery, PagedResult<ArticleSummaryResult>>
{
}
