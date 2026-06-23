using InventoryManagement.Application.Abstractions;
using InventoryManagement.Application.Articles.SearchStockBuckets;
using InventoryManagement.Application.Articles.Shared;

namespace InventoryManagement.Application.Ports.In;

public interface ISearchStockBucketsUseCase
    : IUseCase<SearchStockBucketsQuery, IReadOnlyCollection<StockBucketResult>>
{
}
