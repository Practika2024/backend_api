using Application.Commands.Authentications.Exceptions;
using Application.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Api.Modules.Errors;

public static class AuthenticationErrorHandler
{
    public static ObjectResult ToObjectResult(this AuthenticationException exception)
    {
        return new ObjectResult(exception.Message)
        {
            StatusCode = exception switch
            {
                UserByThisEmailAlreadyExistsAuthenticationException => StatusCodes.Status409Conflict,
                EmailOrPasswordAreIncorrectException => StatusCodes.Status400BadRequest,
                AuthenticationUnknownException => StatusCodes.Status500InternalServerError,
                UserNorFoundException => StatusCodes.Status404NotFound,

                InvalidTokenException
                    or TokenExpiredException
                    or InvalidAccessTokenException
                    => StatusCodes.Status426UpgradeRequired,

                _ => throw new NotImplementedException("Authentication error handler does not implemented")
            }
        };
    }
}