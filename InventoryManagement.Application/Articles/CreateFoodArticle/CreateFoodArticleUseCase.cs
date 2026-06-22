using InventoryManagement.Application.Ports.In;
using InventoryManagement.Application.Ports.Out;
using InventoryManagement.Application.Shared;
using InventoryManagement.Domain.Articles;
using InventoryManagement.Domain.Shared.Exceptions;
using InventoryManagement.Domain.Shared.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagement.Application.Articles.CreateFoodArticle
{
    public class CreateFoodArticleUseCase : ICreateFoodArticleUseCase
    {
        private readonly IArticleRepository _articleRepository;
        public CreateFoodArticleUseCase(IArticleRepository articleRepository)
        {
            _articleRepository = articleRepository;
        }

        public async Task<CreateArticleResult> ExecuteAsync(CreateFoodArticleCommand command, CancellationToken cancellationToken = default)
        {
            // nouvelle réference
            var reference = Ean13Reference.Create(command.Reference);

            // Vérification de l'existence de la réference
            var exists = await  _articleRepository.ExistsByReferenceAsync(reference, cancellationToken);

            if (exists)
                throw new BusinessRuleException($"Un article avec la référence '{command.Reference}' existe déjà.");

            var priceExcludingTax = Money.FromDecimal(command.PriceExcludingTax);

            var article = FoodArticle.Create(reference, command.Name, priceExcludingTax, command.SaleModes);

            await _articleRepository.AddAsync(article, cancellationToken);

            return new CreateArticleResult(article.Id, article.Reference.Value, article.Name);
        }
    }
}
