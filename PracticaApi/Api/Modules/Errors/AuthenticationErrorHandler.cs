using Application.Commands.Authentications.Exceptions;
using Application.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Api.Modules.Errors;

public static class AuthenticationErrorHandler
{
    public static IResult ToIResult(this AuthenticationException exception)
    {
        return TypedResults.Problem(exception.Message, statusCode: exception switch
        {
            UserByThisEmailAlreadyExistsAuthenticationException => StatusCodes.Status409Conflict,
            EmailOrPasswordAreIncorrectException => StatusCodes.Status401Unauthorized,
            AuthenticationUnknownException => StatusCodes.Status500InternalServerError,
            UserNorFoundException => StatusCodes.Status404NotFound,
            InvalidTokenException or TokenExpiredException or InvalidAccessTokenException => StatusCodes.Status426UpgradeRequired,
            _ => throw new NotImplementedException("Authentication error handler is not implemented")
        });
    }
}