using Domain.RefreshTokens;
using Domain.RefreshTokens.Models;
using Optional;

namespace Application.Common.Interfaces.Repositories;

public interface IRefreshTokenRepository
{
    Task<Option<RefreshToken>> GetRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken);
    Task<RefreshToken> Create(CreateRefreshTokenModel model, CancellationToken cancellationToken);
    Task MakeAllRefreshTokensExpiredForUser(Guid userId, CancellationToken cancellationToken);
}