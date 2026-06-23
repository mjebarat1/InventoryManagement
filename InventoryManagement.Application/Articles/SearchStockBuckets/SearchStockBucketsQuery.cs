namespace InventoryManagement.Application.Articles.SearchStockBuckets;

public sealed record SearchStockBucketsQuery(Guid ArticleId, string ReferenceDigits);
