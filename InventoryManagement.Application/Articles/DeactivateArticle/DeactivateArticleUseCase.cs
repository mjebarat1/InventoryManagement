using InventoryManagement.Application.Ports.In;
using InventoryManagement.Application.Ports.Out;

namespace InventoryManagement.Application.Articles.DeactivateArticle;

public sealed class DeactivateArticleUseCase : IDeactivateArticleUseCase
{
    private readonly IArticleRepository _articleRepository;

    public DeactivateArticleUseCase(IArticleRepository articleRepository)
    {
        _articleRepository = articleRepository;
    }

    public async Task<bool> ExecuteAsync(
        DeactivateArticleCommand command,
        CancellationToken cancellationToken = default)
    {
        var article = await _articleRepository.GetForUpdateByIdAsync(command.ArticleId, cancellationToken);
        if (article is null)
            return false;

        article.Deactivate();
        await _articleRepository.UpdateAsync(article, cancellationToken);
        return true;
    }
}
