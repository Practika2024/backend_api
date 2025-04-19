using Application.Commands.Users.Exceptions;
using Application.Common;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Repositories;
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
    IFluentEmail fluentEmail) : IRequestHandler<SendEmailConfirmationCommand, Result<User, UserException>>
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
        await fluentEmail.To(user.Email).Subject("Email verification").Body("To verify your email address click here", isHtml: true).SendAsync(cancellationToken);
        
        return user;
    }
}