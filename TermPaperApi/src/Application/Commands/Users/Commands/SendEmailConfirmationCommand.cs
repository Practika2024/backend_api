using Application.Commands.Users.Exceptions;
using Application.Common;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Repositories;
using Application.Services.EmailService;
using Application.Services.EmailVerificationLinkFactory;
using Domain.Users;
using FluentEmail.Core;
using MediatR;

namespace Application.Commands.Users.Commands;

public class SendEmailConfirmationCommand : IRequest<Result<User, UserException>>
{
}

public class EmailConfirmationCommandHandler(
    IUserRepository userRepository,
    IUserProvider userProvider,
    IEmailService emailService,
    IEmailVerificationTokenRepository emailVerificationTokenRepository,
    EmailVerificationLinkFactory emailVerificationLinkFactory) : IRequestHandler<SendEmailConfirmationCommand, Result<User, UserException>>
{
    public async Task<Result<User, UserException>> Handle(SendEmailConfirmationCommand request,
        CancellationToken cancellationToken)
    {
        var userId = userProvider.GetUserId();

        var user = await userRepository.GetById(userId, cancellationToken);

        return await user.Match<Task<Result<User, UserException>>>(
            async u => await ConfirmEmailForUser(u, cancellationToken),
            () => Task.FromResult<Result<User, UserException>>(new UserNotFoundException(userId))
        );
    }

    private async Task<Result<User, UserException>> ConfirmEmailForUser(User user, CancellationToken cancellationToken)
    {
        var verificationToken = await emailVerificationTokenRepository.Create(cancellationToken);
        
        string verificationLink = emailVerificationLinkFactory.Create(verificationToken);
        
        var body = $"To verify your email address <a href='{verificationLink}'>click here</a>";
        
        await emailService.SendEmail(user.Email, "Email verification", body);
        
        return user;
    }
}