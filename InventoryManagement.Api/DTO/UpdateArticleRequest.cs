using System.Text.Json.Serialization;
using InventoryManagement.Domain.Articles;

namespace InventoryManagement.Api.DTO;

[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public sealed record UpdateArticleRequest(
    string Name,
    decimal PriceExcludingTax,
    IReadOnlyCollection<SaleMode>? AllowedSaleModes);
