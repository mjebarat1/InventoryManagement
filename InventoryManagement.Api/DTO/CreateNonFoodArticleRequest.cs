using System.Text.Json.Serialization;

namespace InventoryManagement.Api.DTO;

[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public sealed record CreateNonFoodArticleRequest(
    string Reference,
    string Name,
    decimal PriceExcludingTax);
