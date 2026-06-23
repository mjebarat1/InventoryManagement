using InventoryManagement.Domain.StockMovement;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagement.Infrastructure.Persistence.Configurations.StockMovement
{
    public sealed class StockMovementLineConfiguration : IEntityTypeConfiguration<StockMovementLine>
    {
        public void Configure(EntityTypeBuilder<StockMovementLine> builder)
        {
            builder.ToTable("StockMovementLines");

            builder.HasKey(line => line.Id);

            builder.Property(line => line.StockMovementId)
                .IsRequired();

            builder.Property(line => line.StockBucketId)
                .IsRequired();

            builder.Property(line => line.QuantityDelta)
                .IsRequired();

            builder.OwnsOne(line => line.QuantityBefore, quantity =>
            {
                quantity.Property(x => x.Value)
                    .HasColumnName("QuantityBefore")
                    .IsRequired();
            });

            builder.OwnsOne(line => line.QuantityAfter, quantity =>
            {
                quantity.Property(x => x.Value)
                    .HasColumnName("QuantityAfter")
                    .IsRequired();
            });

            builder.HasOne(line => line.StockBucket)
                .WithMany()
                .HasForeignKey(line => line.StockBucketId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(line => line.StockMovementId);
            builder.HasIndex(line => line.StockBucketId);
        }
    }
}
