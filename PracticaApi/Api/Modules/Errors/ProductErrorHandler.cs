using Application.Commands.Products.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Api.Modules.Errors;

public static class ProductErrorHandler
{
    public static ObjectResult ToObjectResult(this ProductException exception)
    {
        return new ObjectResult(new { Message = exception.Message })
        {
            StatusCode = exception switch
            {
                ProductNotFoundException
                    or ProductTypeNotFoundException
                    or UserNotFoundException => StatusCodes.Status404NotFound,
                ProductAlreadyExistsException => StatusCodes.Status409Conflict,
                ProductUnknownException => StatusCodes.Status500InternalServerError,
                _ => throw new NotImplementedException(
                    "Product error handler does not implemented for this exception type")
            }
        };
    }
}