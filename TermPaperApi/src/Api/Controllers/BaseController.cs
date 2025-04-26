using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

public class BaseController : ControllerBase
{
    protected IActionResult GetResult(ServiceResponse serviseResponse)
    {
        return StatusCode((int)serviseResponse.StatusCode, serviseResponse);
    }
}