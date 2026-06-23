using InventoryManagement.Application.Ports.Out;

namespace InventoryManagement.Application.Articles.SearchArticles;

public sealed record SearchArticlesQuery(
    int PageNumber,
    int PageSize,
    ArticleSortField SortBy,
    SortDirection SortDirection,
    ArticleKind? Type,
    string? SearchTerm,
    ArticleActivityFilter ActivityFilter);
