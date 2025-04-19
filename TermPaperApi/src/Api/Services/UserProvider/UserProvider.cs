using Application.Common.Interfaces;
using Domain.Users;

namespace Api.Services.UserProvider;

public class UserProvider(IHttpContextAccessor context) : IUserProvider
{
    private readonly IHttpContextAccessor _context = context ?? throw new ArgumentNullException(nameof(context));

    public Guid GetUserId()
    {
        var userIdStr = _context.HttpContext!.User.FindFirst("id")?.Value;
        
        var userId = userIdStr != null ? Guid.Parse(userIdStr) : Guid.Empty;
        
        return userId;
    }
}