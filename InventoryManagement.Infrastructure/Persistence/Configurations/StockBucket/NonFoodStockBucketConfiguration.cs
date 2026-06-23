using InventoryManagement.Domain.StockBucket;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Infrastructure.Persistence.Configurations.StockBucket
{
    public sealed class NonFoodStockBucketConfiguration : IEntityTypeConfiguration<NonFoodStockBucket>
    {
        public void Configure(EntityTypeBuilder<NonFoodStockBucket> builder)
        {
            builder.Property(bucket => bucket.PackagingLevel)
                .HasColumnName("PackagingLevel")
                .HasConversion<string>()
                .HasMaxLength(50)
                .IsRequired();
        }
    }
}
