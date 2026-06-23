using InventoryManagement.Application.Articles.CreateFoodArticle;
using InventoryManagement.Application.Ports.In;
using InventoryManagement.Application.Articles.CreateNonFoodArticle;
using InventoryManagement.Application.Articles.GetArticleById;
using InventoryManagement.Application.Articles.SearchArticles;
using InventoryManagement.Application.Articles.RecordSupply;
using InventoryManagement.Application.Articles.RecordSale;
using InventoryManagement.Application.Articles.RecordInventory;
using InventoryManagement.Application.Articles.SearchStockBuckets;
using InventoryManagement.Application.Articles.UpdateArticle;
using InventoryManagement.Application.Articles.DeactivateArticle;
using InventoryManagement.Domain.StockMovement;
using Microsoft.Extensions.DependencyInjection;

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
            services.AddScoped<IRecordSaleUseCase, RecordSaleUseCase>();
            services.AddScoped<IRecordInventoryUseCase, RecordInventoryUseCase>();
            services.AddScoped<ISearchStockBucketsUseCase, SearchStockBucketsUseCase>();
            services.AddScoped<IUpdateArticleUseCase, UpdateArticleUseCase>();
            services.AddScoped<IDeactivateArticleUseCase, DeactivateArticleUseCase>();
            services.AddScoped<StockSaleAllocator>();

            return services;
        }
    }
}
