using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

public class BaseController : ControllerBase
{
    protected IActionResult GetResult(ServiceResponse serviceResponse)
    {
        return StatusCode((int)serviceResponse.StatusCode, serviceResponse);
    }
}