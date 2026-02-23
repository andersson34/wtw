using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using InvoiceService.Api.Helpers;

namespace InvoiceService.Api.Middlewares;

public sealed class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public GlobalExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ApiHttpException ex)
        {
            await WriteError(context, ex.StatusCode, ex.Message, ex.Errors);
        }
        catch (Exception)
        {
            await WriteError(context, StatusCodes.Status500InternalServerError, "Error inesperado.", Array.Empty<string>());
        }
    }

    private static async Task WriteError(HttpContext context, int statusCode, string message, IReadOnlyList<string> errors)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        var payload = new ApiResponse<object?>
        {
            Success = false,
            Data = null,
            Message = message,
            Errors = errors
        };

        var json = JsonSerializer.Serialize(payload, new JsonSerializerOptions(JsonSerializerDefaults.Web));
        await context.Response.WriteAsync(json);
    }
}
