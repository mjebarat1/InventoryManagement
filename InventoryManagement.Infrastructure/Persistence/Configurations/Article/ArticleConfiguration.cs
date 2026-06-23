using InventoryManagement.Domain.Articles;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Infrastructure.Persistence.Configurations.Article
{
    public sealed class ArticleConfiguration : IEntityTypeConfiguration<Domain.Articles.Article>
    {
        public void Configure(EntityTypeBuilder<Domain.Articles.Article> builder)
        {
            builder.ToTable("Articles");

            builder.HasKey(article => article.Id);

            builder.Property(article => article.Name)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(article => article.IsActive)
                .HasDefaultValue(true)
                .IsRequired();

            builder.OwnsOne(article => article.Reference, reference =>
            {
                reference.Property(x => x.Value)
                    .HasColumnName("Reference")
                    .HasMaxLength(13)
                    .IsRequired();

                reference.HasIndex(x => x.Value)
                    .IsUnique();
            });

            builder.OwnsOne(article => article.PriceExcludingTax, money =>
            {
                money.Property(x => x.Amount)
                    .HasColumnName("PriceExcludingTax")
                    .HasPrecision(18, 2)
                    .IsRequired();
            });

            builder.HasDiscriminator<string>("ArticleType")
                .HasValue<FoodArticle>("Food")
                .HasValue<NonFoodArticle>("NonFood");

        }
    }
}
