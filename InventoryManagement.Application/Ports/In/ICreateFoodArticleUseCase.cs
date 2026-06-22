using InventoryManagement.Application.Abstractions;
using InventoryManagement.Application.Articles.CreateFoodArticle;
using InventoryManagement.Application.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagement.Application.Ports.In
{
    public interface ICreateFoodArticleUseCase : IUseCase<CreateFoodArticleCommand, CreateArticleResult>
    {

    }
}
