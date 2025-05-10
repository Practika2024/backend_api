using System.Net;
using System.Text.Json;
using Application.Common.Interfaces;
using Application.Services;
using Microsoft.AspNetCore.Http;

namespace Application.Middlewares;

public class UserValidationMiddleware(RequestDelegate next)
{
    public async Task Invoke(HttpContext context, IUserProvider userProvider)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var user = await userProvider.GetUserByIdAsync(userProvider.GetUserId(), cancellationToken: default);
            if (user == null || user.IsApprovedByAdmin == false)
            {
                var response = ServiceResponse.GetResponse("Access denied. User is not approved.", false, null,
                    HttpStatusCode.Forbidden);
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
                return;
            }
        }

        await next(context);
    }
}