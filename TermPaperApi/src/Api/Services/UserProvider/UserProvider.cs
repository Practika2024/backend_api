using Application.Common.Interfaces;
using Application.Common.Interfaces.Queries;
using Domain.Users;
using Optional.Unsafe;

namespace Api.Services.UserProvider;

public class UserProvider(IHttpContextAccessor context, IUserQueries userQueries) : IUserProvider
{
    private readonly IHttpContextAccessor _context = context ?? throw new ArgumentNullException(nameof(context));

    public Guid GetUserId()
    {
        var userIdStr = _context.HttpContext!.User.FindFirst("id")?.Value;
        
        if (userIdStr == null)
        {
            throw new InvalidOperationException("User ID claim not found.");
        }
        
        return Guid.Parse(userIdStr);
    }
    
    public async Task<User?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        var user = await userQueries.GetByIdAsQuery(userId, cancellationToken);

        return user.ValueOrDefault();
    }
}