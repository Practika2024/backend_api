using Application.Common;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Exceptions;
using Application.Models.UserModels;
using Domain.Users;
using MediatR;
using Optional;

namespace Application.Commands.Users.Commands;
public record ChangeRolesForUserCommand : IRequest<Result<UserEntity, UserException>>
{
    public required Guid UserId { get; init; }
    public List<string> Roles { get; init; } = new();
}

public class ChangeRolesForUserCommandHandler(
    IUserRepository userRepository) : IRequestHandler<ChangeRolesForUserCommand, Result<UserEntity, UserException>>
{
    public async Task<Result<UserEntity, UserException>> Handle(
        ChangeRolesForUserCommand request,
        CancellationToken cancellationToken)
    {
        var userId = request.UserId;
        var existingUser = await userRepository.GetById(userId, cancellationToken);

        return await existingUser.Match(
            async user =>
            {
                try
                {
                    var updateRolesModel = new UpdateRolesModel
                    {
                        UserId = userId,
                        RoleIds = request.Roles
                    };
                    var updatedUser = await userRepository.UpdateRoles(updateRolesModel, cancellationToken);
                    return updatedUser;
                }
                catch (Exception exception)
                {
                    return new UserUnknownException(user.Id, exception);
                }
            },
            () => Task.FromResult<Result<UserEntity, UserException>>(new UserNotFoundException(userId))
        );
    }
}