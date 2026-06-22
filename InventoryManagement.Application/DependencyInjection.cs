using InventoryManagement.Application.Articles.CreateFoodArticle;
using InventoryManagement.Application.Ports.In;
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

            return services;
        }
    }
}
