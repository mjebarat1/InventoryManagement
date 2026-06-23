using InventoryManagement.Application.Ports.In;
using InventoryManagement.Application.Ports.Out;
using InventoryManagement.Application.Shared;
using InventoryManagement.Domain.Articles;
using InventoryManagement.Domain.Shared.Exceptions;
using InventoryManagement.Domain.Shared.ValueObjects;

namespace InventoryManagement.Application.Articles.CreateFoodArticle;

public sealed class CreateFoodArticleUseCase : ICreateFoodArticleUseCase
{
    private readonly IArticleRepository _articleRepository;

    public CreateFoodArticleUseCase(IArticleRepository articleRepository)
    {
        _articleRepository = articleRepository;
    }

    public async Task<CreateArticleResult> ExecuteAsync(
        CreateFoodArticleCommand command,
        CancellationToken cancellationToken = default)
    {
        var reference = Ean13Reference.Create(command.Reference);

        if (await _articleRepository.ExistsByReferenceAsync(reference, cancellationToken))
        {
            throw new BusinessRuleException(
                DomainErrorCodes.ArticleReferenceAlreadyExists,
                new Dictionary<string, object?> { ["reference"] = command.Reference });
        }

        var article = FoodArticle.Create(
            reference,
            command.Name,
            Money.FromDecimal(command.PriceExcludingTax),
            command.SaleModes);

        await _articleRepository.AddAsync(article, cancellationToken);

        return new CreateArticleResult(article.Id, article.Reference.Value, article.Name);
    }
}
