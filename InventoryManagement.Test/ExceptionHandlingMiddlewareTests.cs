using System.Text.Json;
using InventoryManagement.Api.MiddleWares;
using InventoryManagement.Domain.Shared.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;

namespace InventoryManagement.Test;

public sealed class ExceptionHandlingMiddlewareTests
{
    [Fact]
    public async Task InvokeAsync_ReturnsStructuredBusinessError()
    {
        var exception = new BusinessRuleException(
            DomainErrorCodes.StockInsufficient,
            new Dictionary<string, object?>
            {
                ["requestedQuantity"] = 10,
                ["availableQuantity"] = 6
            });
        var middleware = new ExceptionHandlingMiddleware(
            _ => throw exception,
            NullLogger<ExceptionHandlingMiddleware>.Instance);
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        await middleware.InvokeAsync(context);

        context.Response.Body.Position = 0;
        using var document = await JsonDocument.ParseAsync(context.Response.Body);
        var root = document.RootElement;
        Assert.Equal(StatusCodes.Status400BadRequest, context.Response.StatusCode);
        Assert.Equal("application/problem+json", context.Response.ContentType);
        Assert.Equal(DomainErrorCodes.StockInsufficient, root.GetProperty("code").GetString());
        Assert.Equal(10, root.GetProperty("parameters").GetProperty("requestedQuantity").GetInt32());
        Assert.Equal(6, root.GetProperty("parameters").GetProperty("availableQuantity").GetInt32());
        Assert.False(root.TryGetProperty("detail", out _));
    }
}
