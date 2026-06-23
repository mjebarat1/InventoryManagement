using System.Text.RegularExpressions;
using InventoryManagement.Application.Articles.Shared;
using InventoryManagement.Application.Ports.In;
using InventoryManagement.Application.Ports.Out;
using InventoryManagement.Domain.Shared.Exceptions;

namespace InventoryManagement.Application.Articles.SearchStockBuckets;

public sealed partial class SearchStockBucketsUseCase : ISearchStockBucketsUseCase
{
    private const int MaximumResults = 20;
    private readonly IStockBucketRepository _stockBucketRepository;
    private readonly IClock _clock;

    public SearchStockBucketsUseCase(IStockBucketRepository stockBucketRepository, IClock clock)
    {
        _stockBucketRepository = stockBucketRepository;
        _clock = clock;
    }

    public async Task<IReadOnlyCollection<StockBucketResult>> ExecuteAsync(
        SearchStockBucketsQuery query,
        CancellationToken cancellationToken = default)
    {
        if (query.ArticleId == Guid.Empty)
            throw new BusinessRuleException("L'article est obligatoire.");
        if (!ReferenceDigitsPattern().IsMatch(query.ReferenceDigits ?? string.Empty))
            throw new BusinessRuleException("Saisissez entre 9 et 13 chiffres pour rechercher une référence de lot.");

        var snapshots = await _stockBucketRepository.SearchByReferencePrefixAsync(
            query.ArticleId,
            $"ref-lot-{query.ReferenceDigits}",
            MaximumResults,
            cancellationToken);
        var today = DateOnly.FromDateTime(_clock.Today);

        return snapshots
            .Select(snapshot => ArticleReadModelMapper.ToBucket(
                snapshot.Bucket,
                snapshot.PhysicalQuantity,
                today))
            .ToArray();
    }

    [GeneratedRegex("^[0-9]{9,13}$", RegexOptions.CultureInvariant)]
    private static partial Regex ReferenceDigitsPattern();
}
