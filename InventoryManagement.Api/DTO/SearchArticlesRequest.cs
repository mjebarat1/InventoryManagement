using InventoryManagement.Application.Ports.Out;

namespace InventoryManagement.Api.DTO;

public sealed record SearchArticlesRequest(
    int PageNumber = 1,
    int PageSize = 20,
    ArticleSortField SortBy = ArticleSortField.Reference,
    SortDirection SortDirection = SortDirection.Asc,
    ArticleKind? Type = null,
    string? SearchTerm = null,
    ArticleActivityFilter ActivityFilter = ArticleActivityFilter.Active);
