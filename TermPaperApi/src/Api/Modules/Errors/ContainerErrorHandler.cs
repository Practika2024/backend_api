﻿using Application.Commands.Containers.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Api.Modules.Errors;
public static class ContainerErrorHandler
{
    public static ObjectResult ToObjectResult(this ContainerException exception)
    {
        return new ObjectResult(new { Message = exception.Message })
        {
            StatusCode = exception switch
            {
                ContainerNotFoundException
                    or UserNotFoundException
                    or ContainerTypeNotFoundException => StatusCodes.Status404NotFound,
                ContainerAlreadyExistsException 
                    or ContainerByThisUniqueCodeAlreadyExistsException => StatusCodes.Status409Conflict,
                ContainerCreationException 
                    or ContainerUnknownException => StatusCodes.Status500InternalServerError,
                ProductNotFoundException 
                    or ProductForContainerNotFoundException => StatusCodes.Status404NotFound,
                _ => throw new NotImplementedException("Container error handler does not implemented for this exception type")
            }
        };
    }
}