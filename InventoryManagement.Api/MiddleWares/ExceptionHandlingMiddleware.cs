using InventoryManagement.Domain.Shared.Exceptions;
using System.Net;
using System.Text.Json;

namespace InventoryManagement.Api.MiddleWares
{
    public sealed class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(
            RequestDelegate next,
            ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (BusinessRuleException exception)
            {
                await WriteBusinessRuleProblemDetailsAsync(context, exception);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Unhandled exception");

                await WriteProblemDetailsAsync(
                    context,
                    HttpStatusCode.InternalServerError,
                    "Internal server error",
                    "An unexpected error occurred.");
            }
        }

        private static async Task WriteBusinessRuleProblemDetailsAsync(
            HttpContext context,
            BusinessRuleException exception)
        {
            context.Response.ContentType = "application/problem+json";
            context.Response.StatusCode = StatusCodes.Status400BadRequest;

            var response = new
            {
                type = "https://httpstatuses.com/400",
                title = "Business rule violation",
                status = StatusCodes.Status400BadRequest,
                code = exception.Code,
                parameters = exception.Parameters
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }

        private static async Task WriteProblemDetailsAsync(
            HttpContext context,
            HttpStatusCode statusCode,
            string title,
            string detail)
        {
            context.Response.ContentType = "application/problem+json";
            context.Response.StatusCode = (int)statusCode;

            var response = new
            {
                type = $"https://httpstatuses.com/{(int)statusCode}",
                title,
                status = (int)statusCode,
                detail
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}
