using InventoryManagement.Application.Articles.Shared;
using InventoryManagement.Application.Ports.In;
using InventoryManagement.Application.Ports.Out;

namespace InventoryManagement.Application.Articles.GetArticleById;

public sealed class GetArticleByIdUseCase : IGetArticleByIdUseCase
{
    private readonly IArticleStockReadRepository _articleStockReadRepository;
    private readonly IClock _clock;

    public GetArticleByIdUseCase(IArticleStockReadRepository articleStockReadRepository, IClock clock)
    {
        _articleStockReadRepository = articleStockReadRepository;
        _clock = clock;
    }

    public async Task<ArticleDetailsResult?> ExecuteAsync(
        GetArticleByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        if (query.ArticleId == Guid.Empty)
            return null;

        var snapshot = await _articleStockReadRepository.GetByArticleIdAsync(query.ArticleId, cancellationToken);
        return snapshot is null
            ? null
            : ArticleReadModelMapper.ToDetails(snapshot, DateOnly.FromDateTime(_clock.Today));
    }
}
