using InventoryManagement.Domain.Articles;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagement.Infrastructure.Persistence.Configurations.Article
{
    public sealed class NonFoodArticleConfiguration : IEntityTypeConfiguration<NonFoodArticle>
    {
        public void Configure(EntityTypeBuilder<NonFoodArticle> builder)
        {
        }
    }
}
