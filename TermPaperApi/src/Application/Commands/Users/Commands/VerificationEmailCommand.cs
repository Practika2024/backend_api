using System.Net;
using Application.Commands.Users.Exceptions;
using Application.Common;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Repositories;
using Application.Services;
using Application.Services.EmailVerificationLinkFactory;
using Domain.EmailVerificationToken;
using Domain.Users;
using FluentEmail.Core;
using MediatR;

namespace Application.Commands.Users.Commands;

public class VerificationEmailCommand : IRequest<ServiceResponse>
{
    public required Guid TokenId { get; init; }
}

public class VerificationEmailCommandCommandHandler(
    IUserRepository userRepository,
    IEmailVerificationTokenRepository emailVerificationTokenRepository) : IRequestHandler<VerificationEmailCommand, ServiceResponse>
{
    public async Task<ServiceResponse> Handle(VerificationEmailCommand request,
        CancellationToken cancellationToken)
    {
                
        EmailVerificationToken? verificationToken = await emailVerificationTokenRepository.Get(request.TokenId, cancellationToken);

        if (verificationToken is null)
        {
            return ServiceResponse.NotFoundResponse("Email verification token not found");
        }
        
        var user = await userRepository.FindUserByEmailVerificationToken(request.TokenId, cancellationToken);
        
        if (verificationToken.ExpiresOnUtc < DateTime.UtcNow || user.EmailConfirmed)
        {
            return ServiceResponse.GetResponse("Email verification token has expired", false, null, HttpStatusCode.Conflict);
        }

        if (user is null)
        {
            return ServiceResponse.NotFoundResponse("User not found");
        }


        await userRepository.VerifyEmailUser(user.Id,cancellationToken);

        await emailVerificationTokenRepository.Delete(request.TokenId, cancellationToken);

        return ServiceResponse.OkResponse("Email confirmed");
    }
}