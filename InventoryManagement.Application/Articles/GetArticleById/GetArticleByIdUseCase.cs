using InventoryManagement.Application.Articles.Shared;
using InventoryManagement.Application.Ports.In;
using InventoryManagement.Application.Ports.Out;

namespace InventoryManagement.Application.Articles.GetArticleById;

public sealed class GetArticleByIdUseCase : IGetArticleByIdUseCase
{
    private readonly IArticleRepository _articleRepository;

    public GetArticleByIdUseCase(IArticleRepository articleRepository) => _articleRepository = articleRepository;

    public async Task<ArticleDetailsResult?> ExecuteAsync(
        GetArticleByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        if (query.ArticleId == Guid.Empty)
            return null;

        var article = await _articleRepository.GetByIdAsync(query.ArticleId, cancellationToken);
        return article is null ? null : ArticleReadModelMapper.ToDetails(article);
    }
}
