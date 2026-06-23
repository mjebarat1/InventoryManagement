using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using InventoryManagement.Domain.StockMovement;

namespace InventoryManagement.Infrastructure.Persistence.Configurations.StockMovement
{
    public sealed class SaleMovementConfiguration : IEntityTypeConfiguration<SaleMovement>
    {
        public void Configure(EntityTypeBuilder<SaleMovement> builder)
        {
            builder.OwnsOne(movement => movement.UnitPriceExcludingTax, money =>
            {
                money.Property(x => x.Amount)
                    .HasColumnName("UnitPriceExcludingTax")
                    .HasPrecision(18, 2)
                    .IsRequired();
            });

            builder.OwnsOne(movement => movement.UnitPriceIncludingTax, money =>
            {
                money.Property(x => x.Amount)
                    .HasColumnName("UnitPriceIncludingTax")
                    .HasPrecision(18, 2)
                    .IsRequired();
            });

            builder.OwnsOne(movement => movement.VatRate, vatRate =>
            {
                vatRate.Property(x => x.Value)
                    .HasColumnName("VatRate")
                    .HasPrecision(5, 3)
                    .IsRequired();
            });

            builder.Navigation(movement => movement.UnitPriceExcludingTax)
                .IsRequired();

            builder.Navigation(movement => movement.UnitPriceIncludingTax)
                .IsRequired();

            builder.Navigation(movement => movement.VatRate)
                .IsRequired();
        }
    }
}
