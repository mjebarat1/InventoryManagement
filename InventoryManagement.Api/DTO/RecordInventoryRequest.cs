using System.Text.Json.Serialization;
using InventoryManagement.Domain.Articles;

namespace InventoryManagement.Api.DTO;

[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public sealed record RecordInventoryRequest(
    string? Comment,
    IReadOnlyCollection<RecordInventoryExistingBucketRequest>? ExistingBuckets,
    IReadOnlyCollection<RecordInventoryNewBucketRequest>? NewBuckets);

public sealed record RecordInventoryExistingBucketRequest(
    Guid StockBucketId,
    int CountedQuantity);

public sealed record RecordInventoryNewBucketRequest(
    string Reference,
    int CountedQuantity,
    DateOnly? ExpirationDate,
    PackagingLevel? PackagingLevel);
