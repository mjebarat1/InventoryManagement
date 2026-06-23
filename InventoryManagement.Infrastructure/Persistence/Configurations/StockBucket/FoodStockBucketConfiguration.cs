using InventoryManagement.Domain.StockBucket;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;


namespace InventoryManagement.Infrastructure.Persistence.Configurations.StockBucket
{
    public sealed class FoodStockBucketConfiguration : IEntityTypeConfiguration<FoodStockBucket>
    {
        public void Configure(EntityTypeBuilder<FoodStockBucket> builder)
        {
            builder.Property(bucket => bucket.ExpirationDate)
                .HasColumnName("ExpirationDate")
                .IsRequired();
        }
    }
}
