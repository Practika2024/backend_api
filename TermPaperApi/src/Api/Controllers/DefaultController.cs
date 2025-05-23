using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("")]
[ApiController]
public class DefaultController : ControllerBase
{
    [HttpGet]
    public IActionResult Index()
    {
        return Redirect("/swagger/index.html");
    }
}