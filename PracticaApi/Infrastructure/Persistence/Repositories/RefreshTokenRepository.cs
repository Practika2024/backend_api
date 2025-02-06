using Application.Common.Interfaces.Repositories;
using Domain.Authentications.Users;
using Domain.RefreshTokens;
using Microsoft.EntityFrameworkCore;
using Optional;

namespace Infrastructure.Persistence.Repositories;

public class RefreshTokenRepository(ApplicationDbContext context) : IRefreshTokenRepository
{
    public async Task<Option<RefreshToken>> GetRefreshTokenAsync(string refreshToken,
        CancellationToken cancellationToken)
    {
        var entity = await context.RefreshTokens
            .FirstOrDefaultAsync(t => t.Token == refreshToken, cancellationToken);

        return entity == null ? Option.None<RefreshToken>() : Option.Some(entity);
    }

    public async Task<RefreshToken> Create(RefreshToken refreshToken, CancellationToken cancellationToken)
    {
        await context.RefreshTokens.AddAsync(refreshToken, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return refreshToken;
    }

    public async Task MakeAllRefreshTokensExpiredForUser(UserId userId, CancellationToken cancellationToken)
    {
        var refreshTokens = context.RefreshTokens.Where(t => t.UserId == userId);

        if (!await refreshTokens.AnyAsync(cancellationToken))
        {
            return;
        }
        
        await refreshTokens.ForEachAsync(t => { t.IsUsed = true; }, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);
    }
}