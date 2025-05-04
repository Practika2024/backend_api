using Domain.EmailVerificationToken;
using Optional;

namespace Application.Common.Interfaces.Repositories;

public interface IEmailVerificationTokenRepository
{
    Task<EmailVerificationToken> Create(CancellationToken cancellationToken);
    Task<EmailVerificationToken> Get(Guid tokenId, CancellationToken cancellationToken);
    Task<bool> Delete(Guid tokenId, CancellationToken cancellationToken);
}