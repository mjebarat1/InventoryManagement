using InventoryManagement.Domain.Articles;

namespace InventoryManagement.Application.Articles.RecordInventory;

public sealed record RecordInventoryExistingBucketCommand(
    Guid StockBucketId,
    int CountedQuantity);

public sealed record RecordInventoryNewBucketCommand(
    string Reference,
    int CountedQuantity,
    DateOnly? ExpirationDate,
    PackagingLevel? PackagingLevel);

public sealed record RecordInventoryCommand(
    Guid ArticleId,
    string? Comment,
    IReadOnlyCollection<RecordInventoryExistingBucketCommand> ExistingBuckets,
    IReadOnlyCollection<RecordInventoryNewBucketCommand> NewBuckets);

public sealed record RecordInventoryResult(
    Guid MovementId,
    int AdjustedBucketCount,
    int CreatedBucketCount);
