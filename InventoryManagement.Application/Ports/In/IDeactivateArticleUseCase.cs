using InventoryManagement.Application.Abstractions;
using InventoryManagement.Application.Articles.DeactivateArticle;

namespace InventoryManagement.Application.Ports.In;

public interface IDeactivateArticleUseCase : IUseCase<DeactivateArticleCommand, bool>
{
}
