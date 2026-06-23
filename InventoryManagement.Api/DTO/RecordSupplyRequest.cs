using InventoryManagement.Domain.Articles;
using System.Text.Json.Serialization;

namespace InventoryManagement.Api.DTO;

[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public sealed record RecordSupplyRequest(
    int Quantity,
    DateOnly? ExpirationDate,
    PackagingLevel? PackagingLevel);
