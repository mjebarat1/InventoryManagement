using InventoryManagement.Domain.Articles;

namespace InventoryManagement.Application.Articles.RecordSupply;

public sealed record RecordSupplyCommand(
    Guid ArticleId,
    int Quantity,
    DateOnly? ExpirationDate,
    PackagingLevel? PackagingLevel);

public sealed record RecordSupplyResult(
    Guid MovementId,
    Guid BucketId);
