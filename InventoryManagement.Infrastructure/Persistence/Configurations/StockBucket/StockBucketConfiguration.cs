using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using InventoryManagement.Domain.StockBucket;

namespace InventoryManagement.Infrastructure.Persistence.Configurations.StockBucket
{
    public sealed class StockBucketConfiguration : IEntityTypeConfiguration<Domain.StockBucket.StockBucket>
    {
        public void Configure(EntityTypeBuilder<Domain.StockBucket.StockBucket> builder)
        {
            builder.ToTable("StockBuckets");

            builder.HasKey(bucket => bucket.Id);

            builder.Property(bucket => bucket.ArticleId)
                .IsRequired();

            builder.OwnsOne(bucket => bucket.Reference, reference =>
            {
                reference.Property(value => value.Value)
                    .HasColumnName("Reference")
                    .HasMaxLength(21)
                    .IsRequired();
                reference.HasIndex(value => value.Value)
                    .IsUnique();
            });

            builder.Navigation(bucket => bucket.Reference)
                .IsRequired();

            builder.Property(bucket => bucket.CreatedAt)
                .IsRequired();


            builder.HasOne(bucket => bucket.Article)
                .WithMany()
                .HasForeignKey(bucket => bucket.ArticleId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasDiscriminator<string>("BucketType")
                .HasValue<FoodStockBucket>("Food")
                .HasValue<NonFoodStockBucket>("NonFood");

            builder.HasIndex(bucket => bucket.ArticleId);
        }
    }
}
