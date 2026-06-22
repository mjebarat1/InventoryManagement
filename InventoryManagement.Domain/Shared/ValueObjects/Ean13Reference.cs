using InventoryManagement.Domain.Shared.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagement.Domain.Shared.ValueObjects
{
    public sealed class Ean13Reference
    {
        public string Value { get; }

        private Ean13Reference(string value)
        {
            Value = value;
        }

        public static Ean13Reference Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new BusinessRuleException("La référence EAN-13 est obligatoire.");

            value = value.Trim();

            if (value.Length != 13 || !value.All(char.IsDigit))
                throw new BusinessRuleException("La référence doit contenir exactement 13 chiffres.");

            return new Ean13Reference(value);
        }

        public override string ToString()
        {
            return Value;
        }
    }
}
