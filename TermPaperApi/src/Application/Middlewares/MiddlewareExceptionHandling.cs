using System.Text.Json;
using Application.Services;
using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace Application.Middlewares;

public class MiddlewareExceptionHandling(RequestDelegate next)
{
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (ValidationException ex)
        {
            await context.Response.WriteJsonResponseAsync(StatusCodes.Status400BadRequest,
                ServiceResponse.BadRequestResponse(ex.Message ?? throw new ArgumentNullException(nameof(ex))));
        }
        catch (Exception ex)
        {
            await context.Response.WriteJsonResponseAsync(StatusCodes.Status500InternalServerError,
                ServiceResponse.InternalServerErrorResponse(ex.Message));
        }
    }
}