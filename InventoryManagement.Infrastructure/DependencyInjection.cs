using InventoryManagement.Application.Articles.CreateFoodArticle;
using InventoryManagement.Application.Ports.In;
using InventoryManagement.Application.Ports.Out;
using InventoryManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagement.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddDbContext<StockDbContext>(options =>
            {
                options.UseSqlite(
                    configuration.GetConnectionString("DefaultConnection"));

            });

            services.AddScoped<IArticleRepository, ArticleRepository>();
            services.AddScoped<IArticleStockReadRepository, ArticleStockReadRepository>();
            services.AddScoped<IStockMovementRepository, StockMovementRepository>();
            services.AddScoped<IStockBucketRepository, StockBucketRepository>();

            services.AddSingleton<IClock, SystemClock>();

            return services;
        }
    }
}
