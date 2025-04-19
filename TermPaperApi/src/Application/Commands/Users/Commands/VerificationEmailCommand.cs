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
        var user = await userRepository.FindUserByEmailVerificationToken(request.TokenId, cancellationToken);

        if (user is null)
        {
            return new UserNotFoundException(user.Id);
        }

        return await ConfirmEmailForUser(user, request.TokenId, cancellationToken);
        
        // var user = await userRepository.GetById(userId, cancellationToken);
        //
        // return await user.Match<Task<Result<User, UserException>>>(
        //     async u => await ConfirmEmailForUser(u, request.TokenId, cancellationToken),
        //     () => Task.FromResult<Result<User, UserException>>(new UserNotFoundException(userId))
        // );
    }

    private async Task<Result<User, UserException>> ConfirmEmailForUser(User user, Guid tokenId, CancellationToken cancellationToken)
    {
        EmailVerificationToken? verificationToken = await emailVerificationTokenRepository.Get(tokenId, cancellationToken);

        if (verificationToken is null || verificationToken.ExpiresOnUtc < DateTime.UtcNow || user.EmailConfirmed)
        {
            return new EmailVerificationException(user.Id);
        }

        await userRepository.VerifyEmailUser(user.Id,cancellationToken);

        await emailVerificationTokenRepository.Delete(tokenId, cancellationToken);

        return user;
    }
}