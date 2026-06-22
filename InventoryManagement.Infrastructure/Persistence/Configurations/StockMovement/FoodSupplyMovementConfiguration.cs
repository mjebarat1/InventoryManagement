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
    public sealed class FoodSupplyMovementConfiguration : IEntityTypeConfiguration<FoodSupplyMovement>
    {
        public void Configure(EntityTypeBuilder<FoodSupplyMovement> builder)
        {
            builder.Property(movement => movement.ExpirationDate)
                .HasColumnName("ExpirationDate")
                .IsRequired();
        }
    }
}
