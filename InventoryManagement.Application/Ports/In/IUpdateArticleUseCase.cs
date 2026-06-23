using InventoryManagement.Application.Abstractions;
using InventoryManagement.Application.Articles.UpdateArticle;

namespace InventoryManagement.Application.Ports.In;

public interface IUpdateArticleUseCase : IUseCase<UpdateArticleCommand, bool>
{
}
