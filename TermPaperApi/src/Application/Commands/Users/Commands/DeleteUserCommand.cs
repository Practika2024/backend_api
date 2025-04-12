using Application.Commands.Users.Exceptions;
using Application.Common;
using Application.Common.Interfaces.Repositories;
using Domain.Users;
using Domain.Users.Models;
using MediatR;
using Microsoft.AspNetCore.Hosting;

namespace Application.Commands.Users.Commands;

public record DeleteUserCommand : IRequest<Result<User, UserException>>
{
    public required Guid UserId { get; init; }
}

public class DeleteUserCommandHandler(
    IUserRepository userRepository, IWebHostEnvironment webHostEnvironment)
    : IRequestHandler<DeleteUserCommand, Result<User, UserException>>
{
    public async Task<Result<User, UserException>> Handle(
        DeleteUserCommand request,
        CancellationToken cancellationToken)
    {
        var userId = request.UserId;

        var existingUser = await userRepository.GetById(userId, cancellationToken);

        return await existingUser.Match<Task<Result<User, UserException>>>(
            async user =>
            {
                try
                {
                    var deleteModel = new DeleteUserModel { Id = userId };
                    var deletedUser = await userRepository.Delete(deleteModel, cancellationToken);
                    return deletedUser;
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