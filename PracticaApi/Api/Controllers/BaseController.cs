using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

public abstract class BaseController : ControllerBase
{        
    protected Guid? GetUserId()
    {
        var userIdStr = User.FindFirst("id")?.Value;
        
        var userId = userIdStr != null ? Guid.Parse(userIdStr) : Guid.Empty;
        
        return userId;
    }
}