using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InventoryManagement.Domain.StockMovement;

namespace InventoryManagement.Infrastructure.Persistence.Configurations.StockMovement
{
    public sealed class StockMovementConfiguration : IEntityTypeConfiguration<Domain.StockMovement.StockMovement>
    {
        public void Configure(EntityTypeBuilder<Domain.StockMovement.StockMovement> builder)
        {
            builder.ToTable("ArticleMovements");

            builder.HasKey(movement => movement.Id);

            builder.Property(movement => movement.ArticleId)
                .IsRequired();


            builder.Property(movement => movement.CreatedAt)
                .IsRequired();

            builder.HasDiscriminator<string>("MovementType")
                .HasValue<SupplyMovement>("Supply")
                .HasValue<SaleMovement>("Sale")
                .HasValue<InventoryMovement>("Inventory");

            builder.HasOne(x => x.Article)
                .WithMany(x => x.StockMovements)
                .HasForeignKey(movement => movement.ArticleId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(movement => movement.Lines)
            .WithOne( x => x.StockMovement)
            .HasForeignKey(line => line.StockMovementId)
            .OnDelete(DeleteBehavior.Cascade);

            builder.Navigation(movement => movement.Lines)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

            builder.HasIndex(movement => movement.ArticleId);
            builder.HasIndex(movement => movement.CreatedAt);
        }
    }
}
