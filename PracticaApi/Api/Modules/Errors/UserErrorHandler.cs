using Application.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Api.Modules.Errors;

public static class UserErrorHandler
{
    public static ObjectResult ToObjectResult(this UserException exception)
    {
        return new ObjectResult(exception.Message)
        {
            StatusCode = exception switch
            {
                UserByThisEmailAlreadyExistsException =>
                    StatusCodes.Status409Conflict,

                UserNotFoundException
                    or RoleNotFoundException =>
                    StatusCodes.Status404NotFound,

                EmailOrPasswordAreIncorrect => StatusCodes.Status401Unauthorized,
                
                UserUnknownException or ImageSaveException => StatusCodes.Status500InternalServerError,
                
                _ => throw new NotImplementedException("Authentication error handler does not implemented")
            }
        };
    }
}