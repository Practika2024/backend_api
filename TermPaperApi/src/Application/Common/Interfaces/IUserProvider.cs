using Domain.Users;

namespace Application.Common.Interfaces;

public interface IUserProvider
{
    Guid GetUserId();
    Task<User?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken);
}