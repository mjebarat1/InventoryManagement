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
    public sealed class NonFoodSupplyMovementConfiguration
        : IEntityTypeConfiguration<NonFoodSupplyMovement>
    {
        public void Configure(EntityTypeBuilder<NonFoodSupplyMovement> builder)
        {
            builder.Property(movement => movement.PackagingLevel)
                .HasColumnName("PackagingLevel")
                .HasConversion<string>()
                .HasMaxLength(50);
        }
    }
}
