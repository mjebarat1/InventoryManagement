using System.Text.RegularExpressions;
using InventoryManagement.Domain.Shared.Exceptions;

namespace InventoryManagement.Domain.Shared.ValueObjects;

public sealed partial class StockBucketReference : IEquatable<StockBucketReference>
{
    public string Value { get; }
    private StockBucketReference() => Value = string.Empty;
    private StockBucketReference(string value) => Value = value;

    public static StockBucketReference Create(string value)
    {
        var normalizedValue = value?.Trim() ?? string.Empty;
        if (!ReferencePattern().IsMatch(normalizedValue))
            throw new BusinessRuleException(DomainErrorCodes.StockBucketReferenceInvalid);

        return new StockBucketReference(normalizedValue);
    }

    public bool Equals(StockBucketReference? other) => other is not null && Value == other.Value;
    public override bool Equals(object? obj) => Equals(obj as StockBucketReference);
    public override int GetHashCode() => Value.GetHashCode(StringComparison.Ordinal);
    public override string ToString() => Value;

    [GeneratedRegex("^ref-lot-[0-9]{13}$", RegexOptions.CultureInvariant)]
    private static partial Regex ReferencePattern();
}
