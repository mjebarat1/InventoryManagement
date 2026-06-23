using InventoryManagement.Application.Ports.In;
using InventoryManagement.Application.Ports.Out;
using InventoryManagement.Domain.Articles;
using InventoryManagement.Domain.Shared.Exceptions;
using InventoryManagement.Domain.Shared.ValueObjects;

namespace InventoryManagement.Application.Articles.UpdateArticle;

public sealed class UpdateArticleUseCase : IUpdateArticleUseCase
{
    private readonly IArticleRepository _articleRepository;

    public UpdateArticleUseCase(IArticleRepository articleRepository)
    {
        _articleRepository = articleRepository;
    }

    public async Task<bool> ExecuteAsync(
        UpdateArticleCommand command,
        CancellationToken cancellationToken = default)
    {
        var article = await _articleRepository.GetForUpdateByIdAsync(command.ArticleId, cancellationToken);
        if (article is null)
            return false;

        var price = Money.FromDecimal(command.PriceExcludingTax);
        switch (article)
        {
            case FoodArticle foodArticle:
                if (command.AllowedSaleModes is null)
                    throw new BusinessRuleException("Les modes de vente sont obligatoires pour un article alimentaire.");
                foodArticle.Update(command.Name, price, command.AllowedSaleModes);
                break;
            case NonFoodArticle nonFoodArticle:
                if (command.AllowedSaleModes is { Count: > 0 })
                    throw new BusinessRuleException("Les modes de vente ne doivent pas être renseignés pour un article non alimentaire.");
                nonFoodArticle.Update(command.Name, price);
                break;
            default:
                throw new BusinessRuleException("Type d'article inconnu.");
        }

        await _articleRepository.UpdateAsync(article, cancellationToken);
        return true;
    }
}
