using Application.Common.Interfaces;
using Application.Common.Interfaces.Repositories;
using AutoMapper;
using DataAccessLayer.Data;
using DataAccessLayer.Entities.EmailVerificationTokens;
using Domain.EmailVerificationToken;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Repositories;

public class EmailVerificationTokenRepository(ApplicationDbContext context, IMapper mapper, IUserProvider userProvider)
    : IEmailVerificationTokenRepository
{
    public async Task<EmailVerificationToken> Create(CancellationToken cancellationToken)
    {
        DateTime utcNow = DateTime.UtcNow;

        var verificationToken = new EmailVerificationTokenEntity
        {
            Id = Guid.NewGuid(),
            UserId = userProvider.GetUserId(),
            CreatedOnUtc = utcNow,
            ExpiresOnUtc = utcNow.AddDays(1)
        };

        await context.EmailVerificationTokens.AddAsync(verificationToken, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return mapper.Map<EmailVerificationToken>(verificationToken);
    }

    public async Task<EmailVerificationToken> Get(Guid tokenId, CancellationToken cancellationToken)
    {
        var emailVerificationToken =
            await context.EmailVerificationTokens.FirstOrDefaultAsync(x => x.Id == tokenId, cancellationToken);

        return mapper.Map<EmailVerificationToken>(emailVerificationToken);
    }

    public async Task<bool> Delete(Guid tokenId, CancellationToken cancellationToken)
    {
        var emailVerificationToken =
            await context.EmailVerificationTokens.FirstOrDefaultAsync(e => e.Id == tokenId, cancellationToken);

        if (emailVerificationToken == null)
        {
            return false;
        }

        context.EmailVerificationTokens.Remove(emailVerificationToken);
        await context.SaveChangesAsync(cancellationToken);

        return true;
    }
}