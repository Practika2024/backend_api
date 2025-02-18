using Application.Models.RefreshTokenModels;
using Domain.RefreshTokens;
using Optional;

namespace Application.Common.Interfaces.Repositories
{
    public interface IRefreshTokenRepository
    {
        Task<Option<RefreshTokenEntity>> GetRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken);
        Task<RefreshTokenEntity> Create(CreateRefreshTokenModel model, CancellationToken cancellationToken);
        Task MakeAllRefreshTokensExpiredForUser(Guid userId, CancellationToken cancellationToken);
    }
}