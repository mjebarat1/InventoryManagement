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
    public sealed class FoodArticleConfiguration : IEntityTypeConfiguration<FoodArticle>
    {
        public void Configure(EntityTypeBuilder<FoodArticle> builder)
        {
            builder.Ignore(article => article.SaleModes);

            builder.OwnsMany<FoodArticleSaleMode>("_saleModes", saleModeBuilder =>
            {
                saleModeBuilder.ToTable("FoodArticleSaleModes");

                saleModeBuilder.WithOwner()
                    .HasForeignKey("ArticleId");

                saleModeBuilder.Property(x => x.Value)
                    .HasColumnName("SaleMode")
                    .HasConversion<string>()
                    .HasMaxLength(50)
                    .IsRequired();

                saleModeBuilder.HasKey("ArticleId", "Value");
            });

            builder.Navigation("_saleModes")
                .UsePropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
