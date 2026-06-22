using InventoryManagement.Domain.Shared.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagement.Domain.Shared.ValueObjects
{
    public sealed class Money
    {
        public decimal Amount { get; }

        private Money()
        {
            // Requis par EF Core
        }

        private Money(decimal amount)
        {
            Amount = amount;
        }

        public static Money FromDecimal(decimal amount)
        {
            if (amount < 0)
                throw new BusinessRuleException("Le montant ne peut pas être négatif.");

            return new Money(Math.Round(amount, 2));
        }

        public Money AddVat(VatRate vatRate)
        {
            return FromDecimal(Amount * (1 + vatRate.Value));
        }
    }
}
