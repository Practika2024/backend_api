using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace Application.Middlewares
{
    public class MiddlewareValidationExceptionHandling(RequestDelegate next)
    {
        public async Task Invoke(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (ValidationException ex)
            {
                var response = ex.Message ?? throw new ArgumentNullException(nameof(ex));
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            }
        }
    }
}