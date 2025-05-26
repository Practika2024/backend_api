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
        try
        {
            if (serviceResponse.Payload is not null && serviceResponse.Success)
            {
                serviceResponse.Payload = mapper.Map<T>(serviceResponse.Payload);
            }
        }
        catch (Exception e)
        {
            throw new Exception($"Can't map payload: {e.Message}");
        }
        
        return StatusCode((int)serviceResponse.StatusCode, serviceResponse);
    }
}