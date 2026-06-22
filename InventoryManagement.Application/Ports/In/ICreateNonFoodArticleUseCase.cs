using InventoryManagement.Application.Abstractions;
using InventoryManagement.Application.Articles.CreateNonFoodArticle;
using InventoryManagement.Application.Shared;

namespace InventoryManagement.Application.Ports.In;

public interface ICreateNonFoodArticleUseCase
    : IUseCase<CreateNonFoodArticleCommand, CreateArticleResult>
{
}
