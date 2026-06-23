namespace InventoryManagement.Domain.Shared.Exceptions;

public sealed class BusinessRuleException : Exception
{
    public BusinessRuleException(
        string code,
        IReadOnlyDictionary<string, object?>? parameters = null)
        : base(code)
    {
        Code = code;
        Parameters = parameters is null
            ? new Dictionary<string, object?>()
            : new Dictionary<string, object?>(parameters);
    }

    public string Code { get; }
    public IReadOnlyDictionary<string, object?> Parameters { get; }
}
