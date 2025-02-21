using System.Linq.Expressions;
using Application.Common.Interfaces.Repositories;
using AutoMapper;
using DataAccessLayer.Data;
using DataAccessLayer.Entities.RefreshTokens;
using Domain.RefreshTokenModels;
using Microsoft.EntityFrameworkCore;
using Optional;

namespace DataAccessLayer.Repositories;

public class RefreshTokenRepository(ApplicationDbContext context, IMapper mapper) : IRefreshTokenRepository
{
    public async Task<Option<RefreshToken>> GetRefreshTokenAsync(string refreshToken,
        CancellationToken cancellationToken)
    {
        var entity = await context.RefreshTokens
            .FirstOrDefaultAsync(t => t.Token == refreshToken, cancellationToken);
        
        var token = mapper.Map<RefreshToken>(entity);
        return token == null ? Option.None<RefreshToken>() : Option.Some(token);
    }

    public async Task<RefreshToken> Create(CreateRefreshTokenModel model, CancellationToken cancellationToken)
    {
        var refreshTokenEntity = mapper.Map<RefreshTokenEntity>(model);

        await context.RefreshTokens.AddAsync(refreshTokenEntity, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return mapper.Map<RefreshToken>(refreshTokenEntity);
    }

    public async Task MakeAllRefreshTokensExpiredForUser(Guid userId, CancellationToken cancellationToken)
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