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
                    ServiceResponse.GetResponse("User was not found", false, null,
                        HttpStatusCode.Forbidden));
                return;
            }

            if (user.IsApprovedByAdmin == false)
            {
                await context.Response.WriteJsonResponseAsync(StatusCodes.Status403Forbidden,
                    ServiceResponse.GetResponse("Access denied. User is not approved", false, null,
                        HttpStatusCode.Forbidden));
                return;
            }
        }

        await next(context);
    }
}