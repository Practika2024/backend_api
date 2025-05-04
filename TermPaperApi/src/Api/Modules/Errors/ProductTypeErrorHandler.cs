using Application.Commands.ProductsType.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Api.Modules.Errors;

public static class ProductTypeErrorHandler
{
    public static ObjectResult ToObjectResult(this ProductTypeException exception)
    {
        return new ObjectResult(new { Message = exception.Message })
        {
            StatusCode = exception switch
            {
                ProductTypeNotFoundException => StatusCodes.Status404NotFound,
                ProductTypeAlreadyExistsException => StatusCodes.Status409Conflict,
                ProductUnknownException => StatusCodes.Status500InternalServerError,
                _ => throw new NotImplementedException("Product type error handler does not implemented for this exception type")
            }
        };
    }
}