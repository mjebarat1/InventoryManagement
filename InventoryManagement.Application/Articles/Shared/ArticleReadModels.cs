using InventoryManagement.Application.Ports.Out;
using InventoryManagement.Domain.Articles;

namespace InventoryManagement.Application.Articles.Shared;

public sealed record ArticlePriceResult(SaleMode? SaleMode, decimal VatRate, decimal PriceIncludingTax);

public sealed record StockMovementResult(
    Guid Id,
    DateTime CreatedAt,
    string Type,
    int Quantity,
    DateOnly? ExpirationDate,
    PackagingLevel? PackagingLevel,
    string? Comment);

public sealed record ArticleSummaryResult(
    Guid Id,
    string Reference,
    string Name,
    ArticleKind Type,
    decimal PriceExcludingTax,
    IReadOnlyCollection<ArticlePriceResult> Prices,
    int TotalStock);

public sealed record ArticleDetailsResult(
    Guid Id,
    string Reference,
    string Name,
    ArticleKind Type,
    decimal PriceExcludingTax,
    IReadOnlyCollection<SaleMode> AllowedSaleModes,
    IReadOnlyCollection<ArticlePriceResult> Prices,
    int TotalStock,
    int? SellableStock,
    int? NonSellableStock,
    IReadOnlyCollection<StockMovementResult> Movements);

public sealed record PagedResult<T>(
    IReadOnlyCollection<T> Items,
    int PageNumber,
    int PageSize,
    int TotalItems,
    int TotalPages);
