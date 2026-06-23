using InventoryManagement.Application.Ports.Out;
using InventoryManagement.Domain.Articles;

namespace InventoryManagement.Application.Articles.Shared;

public sealed record ArticlePriceResult(SaleMode? SaleMode, decimal VatRate, decimal PriceIncludingTax);

public sealed record StockMovementResult(
    Guid Id,
    DateTime CreatedAt,
    string Type,
    int QuantityDelta,
    SaleMode? SaleMode,
    int? SoldQuantity,
    decimal? UnitPriceExcludingTax,
    decimal? UnitPriceIncludingTax,
    decimal? VatRate,
    decimal? TotalExcludingTax,
    decimal? TotalIncludingTax,
    string? Comment,
    IReadOnlyCollection<StockMovementLineResult> Lines);

public sealed record StockMovementLineResult(
    Guid Id,
    Guid StockBucketId,
    string StockBucketReference,
    ArticleKind BucketType,
    DateOnly? ExpirationDate,
    PackagingLevel? PackagingLevel,
    int QuantityDelta,
    int? QuantityBefore,
    int? QuantityAfter);

public sealed record StockBucketResult(
    Guid Id,
    string Reference,
    DateTime CreatedAt,
    ArticleKind Type,
    DateOnly? ExpirationDate,
    PackagingLevel? PackagingLevel,
    int PhysicalQuantity,
    int SellableQuantity,
    StockBucketStatus Status);

public enum StockBucketStatus
{
    Empty,
    Sellable,
    Expired,
    Unsellable
}

public sealed record ArticleSummaryResult(
    Guid Id,
    string Reference,
    string Name,
    ArticleKind Type,
    bool IsActive,
    decimal PriceExcludingTax,
    IReadOnlyCollection<ArticlePriceResult> Prices,
    int TotalStock);

public sealed record ArticleDetailsResult(
    Guid Id,
    string Reference,
    string Name,
    ArticleKind Type,
    bool IsActive,
    decimal PriceExcludingTax,
    IReadOnlyCollection<SaleMode> AllowedSaleModes,
    IReadOnlyCollection<ArticlePriceResult> Prices,
    int TotalStock,
    int SellableStock,
    int NonSellableStock,
    IReadOnlyCollection<StockBucketResult> Buckets,
    IReadOnlyCollection<StockMovementResult> Movements);

public sealed record PagedResult<T>(
    IReadOnlyCollection<T> Items,
    int PageNumber,
    int PageSize,
    int TotalItems,
    int TotalPages);
