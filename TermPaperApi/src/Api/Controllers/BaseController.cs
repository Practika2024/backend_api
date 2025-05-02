using Application.Services;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

public class BaseController(IMapper mapper) : ControllerBase
{
    protected IActionResult GetResult(ServiceResponse serviceResponse)
    {
        return StatusCode((int)serviceResponse.StatusCode, serviceResponse);
    }
    
    protected IActionResult GetResult<T>(ServiceResponse serviceResponse)
    {
        if (serviceResponse.Payload is not null)
        {
            serviceResponse.Payload = mapper.Map<T>(serviceResponse.Payload);
        }
        
        return StatusCode((int)serviceResponse.StatusCode, serviceResponse);
    }
}