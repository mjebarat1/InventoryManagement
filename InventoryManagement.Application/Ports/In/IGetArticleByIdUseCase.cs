using InventoryManagement.Application.Abstractions;
using InventoryManagement.Application.Articles.GetArticleById;
using InventoryManagement.Application.Articles.Shared;

namespace InventoryManagement.Application.Ports.In;

public interface IGetArticleByIdUseCase : IUseCase<GetArticleByIdQuery, ArticleDetailsResult?>
{
}
