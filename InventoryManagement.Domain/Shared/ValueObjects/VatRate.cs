using InventoryManagement.Domain.Shared.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagement.Domain.Shared.ValueObjects
{
    public sealed class VatRate
    {
        public decimal Value { get; }

        private VatRate()
        {
            // Requis par EF Core
        }

        private VatRate(decimal value)
        {
            Value = value;
        }

        public static VatRate FromDecimal(decimal value)
        {
            if (value < 0)
                throw new BusinessRuleException("Le taux de TVA ne peut pas être négatif.");

            if (value > 1)
                throw new BusinessRuleException("Le taux de TVA doit être exprimé sous forme décimale, par exemple 0.20 pour 20%.");

            return new VatRate(value);
        }

        public static VatRate FoodTakeAway()
        {
            return new VatRate(0.055m);
        }

        public static VatRate FoodOnSite()
        {
            return new VatRate(0.10m);
        }

        public static VatRate NonFood()
        {
            return new VatRate(0.20m);
        }

        public override string ToString()
        {
            return $"{Value:P1}";
        }
    }
}
