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
            if (user == null)
            {
                await context.Response.WriteJsonResponseAsync(StatusCodes.Status403Forbidden,
                    ServiceResponse.ForbiddenResponse("User was not found"));
                return;
            }

            if (user.IsApprovedByAdmin == false)
            {
                await context.Response.WriteJsonResponseAsync(StatusCodes.Status403Forbidden,
                    ServiceResponse.ForbiddenResponse("Access denied. User is not approved"));
                return;
            }
        }

        await next(context);
    }
}