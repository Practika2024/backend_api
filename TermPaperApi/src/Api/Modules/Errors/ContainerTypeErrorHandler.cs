using Application.Commands.ContainersType.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Api.Modules.Errors;

public static class ContainerTypeErrorHandler
{
    public static ObjectResult ToObjectResult(this ContainerTypeException exception)
    {
        return new ObjectResult(new { Message = exception.Message })
        {
            StatusCode = exception switch
            {
                ContainerTypeNotFoundException => StatusCodes.Status404NotFound,
                ContainerUnknownException => StatusCodes.Status500InternalServerError,
                _ => throw new NotImplementedException("Container type error handler does not implemented for this exception type")
            }
        };
    }
}