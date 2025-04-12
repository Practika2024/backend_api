using Application.Commands.Users.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Api.Modules.Errors;

public static class UserErrorHandler
{
    public static ObjectResult ToObjectResult(this UserException exception)
    {
        return new ObjectResult(new { Message = exception.Message })
        {
            StatusCode = exception switch
            {
                UserByThisEmailAlreadyExistsException => StatusCodes.Status409Conflict,
                EmailOrPasswordAreIncorrect => StatusCodes.Status400BadRequest,
                UserNotFoundException
                    or ImageSaveException
                    or RoleNotFoundException
                    or ProductNotFoundException
                    or UserFavoriteProductNotFoundException => StatusCodes.Status404NotFound,
                ProductAlreadyInFavoritesException => StatusCodes.Status400BadRequest,
                UserUnknownException => StatusCodes.Status500InternalServerError,
                _ => throw new NotImplementedException(
                    "User error handler does not implemented for this exception type")
            }
        };
    }
}