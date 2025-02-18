using Application.Commands.Users.Exceptions;
using Application.Common;
using Application.Common.Interfaces.Repositories;
using Application.Models.UserModels;
using Domain.Users;
using MediatR;
using Microsoft.AspNetCore.Hosting;

namespace Application.Commands.Users.Commands;

public record DeleteUserCommand : IRequest<Result<UserEntity, UserException>>
{
    public required Guid UserId { get; init; }
}

public class DeleteUserCommandHandler(
    IUserRepository userRepository, IWebHostEnvironment webHostEnvironment)
    : IRequestHandler<DeleteUserCommand, Result<UserEntity, UserException>>
{
    public async Task<Result<UserEntity, UserException>> Handle(
        DeleteUserCommand request,
        CancellationToken cancellationToken)
    {
        var userId = request.UserId;

        var existingUser = await userRepository.GetById(userId, cancellationToken);

        return await existingUser.Match<Task<Result<UserEntity, UserException>>>(
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
            () => Task.FromResult<Result<UserEntity, UserException>>(
                new UserNotFoundException(userId))
        );
    }
}