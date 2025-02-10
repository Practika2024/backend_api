using Domain.Authentications.Users;
using Domain.RefreshTokens;
using Optional;

namespace Application.Common.Interfaces.Repositories;

public interface IRefreshTokenRepository
{
    Task<Option<RefreshTokenEntity>> GetRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken);
    Task<RefreshTokenEntity> Create(RefreshTokenEntity refreshTokenEntity, CancellationToken cancellationToken);
    Task MakeAllRefreshTokensExpiredForUser(UserId userId, CancellationToken cancellationToken);
}