using Application.Common.Interfaces.Repositories;
using Application.Models.RefreshTokenModels;
using DataAccessLayer.Data;
using Domain.RefreshTokens;
using Microsoft.EntityFrameworkCore;
using Optional;

namespace DataAccessLayer.Repositories
{
    public class RefreshTokenRepository(ApplicationDbContext context) : IRefreshTokenRepository
    {
        public async Task<Option<RefreshTokenEntity>> GetRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken)
        {
            var entity = await context.RefreshTokens
                .FirstOrDefaultAsync(t => t.Token == refreshToken, cancellationToken);

            return entity == null ? Option.None<RefreshTokenEntity>() : Option.Some(entity);
        }

        public async Task<RefreshTokenEntity> Create(CreateRefreshTokenModel model, CancellationToken cancellationToken)
        {
            var refreshTokenEntity = RefreshTokenEntity.New(
                id: model.Id,
                token: model.Token,
                jwtId: model.JwtId,
                createDate: model.CreateDate,
                expiredDate: model.ExpiredDate,
                userId: model.UserId
            );

            await context.RefreshTokens.AddAsync(refreshTokenEntity, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);

            return refreshTokenEntity;
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
}