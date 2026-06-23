using InventoryManagement.Domain.Shared.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagement.Domain.Shared.ValueObjects
{
    public  class Quantity  : IEquatable<Quantity>
    {
        public int Value { get; }

        private Quantity(int value)
        {
            Value = value;
        }

        public static Quantity Create(int value)
        {
            if (value < 0)
                throw new BusinessRuleException("La quantité ne peut pas être négative.");

            return new Quantity(value);
        }

        public static Quantity CreatePositive(int value)
        {
            if (value <= 0)
                throw new BusinessRuleException("La quantité doit être strictement positive.");

            return new Quantity(value);
        }

        public static Quantity operator +(Quantity left, Quantity right)
        {
            return new Quantity(left.Value + right.Value);
        }

        public static Quantity operator -(Quantity left, Quantity right)
        {
            if (right.Value > left.Value)
                throw new BusinessRuleException("Stock insuffisant.");

            return new Quantity(left.Value - right.Value);
        }

        public bool Equals(Quantity? other)
        {
            if (other is null)
                return false;

            return Value == other.Value;
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as Quantity);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override string ToString()
        {
            return Value.ToString();
        }


    }
}
