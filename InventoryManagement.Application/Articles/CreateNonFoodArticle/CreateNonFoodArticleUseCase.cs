using InventoryManagement.Application.Ports.In;
using InventoryManagement.Application.Ports.Out;
using InventoryManagement.Application.Shared;
using InventoryManagement.Domain.Articles;
using InventoryManagement.Domain.Shared.Exceptions;
using InventoryManagement.Domain.Shared.ValueObjects;

namespace InventoryManagement.Application.Articles.CreateNonFoodArticle;

public sealed class CreateNonFoodArticleUseCase : ICreateNonFoodArticleUseCase
{
    private readonly IArticleRepository _articleRepository;

    public CreateNonFoodArticleUseCase(IArticleRepository articleRepository)
    {
        _articleRepository = articleRepository;
    }

    public async Task<CreateArticleResult> ExecuteAsync(
        CreateNonFoodArticleCommand command,
        CancellationToken cancellationToken = default)
    {
        var reference = Ean13Reference.Create(command.Reference);

        if (await _articleRepository.ExistsByReferenceAsync(reference, cancellationToken))
            throw new BusinessRuleException($"Un article avec la référence '{command.Reference}' existe déjà.");

        var article = NonFoodArticle.Create(
            reference,
            command.Name,
            Money.FromDecimal(command.PriceExcludingTax));

        await _articleRepository.AddAsync(article, cancellationToken);

        return new CreateArticleResult(article.Id, article.Reference.Value, article.Name);
    }
}
