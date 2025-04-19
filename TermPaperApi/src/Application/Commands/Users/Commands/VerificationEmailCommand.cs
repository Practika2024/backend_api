using Application.Commands.Users.Exceptions;
using Application.Common;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Repositories;
using Application.Services.EmailVerificationLinkFactory;
using Domain.EmailVerificationToken;
using Domain.Users;
using FluentEmail.Core;
using MediatR;

namespace Application.Commands.Users.Commands;

public class VerificationEmailCommand : IRequest<Result<User, UserException>>
{
    public required Guid TokenId { get; init; }
}

public class VerificationEmailCommandCommandHandler(
    IUserRepository userRepository,
    IEmailVerificationTokenRepository emailVerificationTokenRepository) : IRequestHandler<VerificationEmailCommand, Result<User, UserException>>
{
    public async Task<Result<User, UserException>> Handle(VerificationEmailCommand request,
        CancellationToken cancellationToken)
    {
                
        EmailVerificationToken? verificationToken = await emailVerificationTokenRepository.Get(request.TokenId, cancellationToken);

        if (verificationToken is null)
        {
            return new EmailVerificationNotFoundException();
        }
        
        var user = await userRepository.FindUserByEmailVerificationToken(request.TokenId, cancellationToken);
        
        if (verificationToken.ExpiresOnUtc < DateTime.UtcNow || user.EmailConfirmed)
        {
            return new EmailVerificationException(user.Id);
        }

        if (user is null)
        {
            return new UserNotFoundException(user.Id);
        }


        await userRepository.VerifyEmailUser(user.Id,cancellationToken);

        await emailVerificationTokenRepository.Delete(request.TokenId, cancellationToken);

        return user;
    }
}