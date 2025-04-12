using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace Application.Middlewares;

public class MiddlewareExceptionHandling
{
    private readonly RequestDelegate _next;

    public MiddlewareExceptionHandling(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException ex)
        {
            var response = ex.Message ?? throw new ArgumentNullException(nameof(ex));
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
        catch (Exception ex)
        {
            var response = ex.Message;
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}