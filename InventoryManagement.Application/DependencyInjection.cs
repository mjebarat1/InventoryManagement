using InventoryManagement.Application.Articles.CreateFoodArticle;
using InventoryManagement.Application.Ports.In;
using InventoryManagement.Application.Articles.CreateNonFoodArticle;
using InventoryManagement.Application.Articles.GetArticleById;
using InventoryManagement.Application.Articles.SearchArticles;
using InventoryManagement.Application.Articles.RecordSupply;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagement.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<ICreateFoodArticleUseCase, CreateFoodArticleUseCase>();
            services.AddScoped<ICreateNonFoodArticleUseCase, CreateNonFoodArticleUseCase>();
            services.AddScoped<IGetArticleByIdUseCase, GetArticleByIdUseCase>();
            services.AddScoped<ISearchArticlesUseCase, SearchArticlesUseCase>();
            services.AddScoped<IRecordSupplyUseCase, RecordSupplyUseCase>();

            return services;
        }
    }
}
