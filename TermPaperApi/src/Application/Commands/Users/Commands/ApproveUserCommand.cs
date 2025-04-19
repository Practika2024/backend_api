using Application.Commands.Users.Exceptions;
using Application.Common;
using Application.Common.Interfaces.Repositories;
using Domain.Users;
using Domain.Users.Models;
using MediatR;
using Microsoft.AspNetCore.Hosting;

namespace Application.Commands.Users.Commands;

public record ApproveUserCommand : IRequest<Result<User, UserException>>
{
    public required Guid UserId { get; init; }
}

public class ApproveUserCommandHandler(
    IUserRepository userRepository)
    : IRequestHandler<ApproveUserCommand, Result<User, UserException>>
{
    public async Task<Result<User, UserException>> Handle(
        ApproveUserCommand request,
        CancellationToken cancellationToken)
    {
        var userId = request.UserId;

        var existingUser = await userRepository.GetById(userId, cancellationToken);

        return await existingUser.Match<Task<Result<User, UserException>>>(
            async user =>
            {
                try
                {
                    var approvedUser = await userRepository.ApproveUser(user.Id, cancellationToken);
                    return approvedUser;
                }
                catch (Exception exception)
                {
                    return new UserUnknownException(userId, exception);
                }
            },
            () => Task.FromResult<Result<User, UserException>>(
                new UserNotFoundException(userId))
        );
    }
}