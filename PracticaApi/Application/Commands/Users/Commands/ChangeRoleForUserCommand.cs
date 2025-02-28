using Application.Common;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Exceptions;
using Domain.Users;
using Domain.Users.Models;
using MediatR;

namespace Application.Commands.Users.Commands;
public record ChangeRoleForUserCommand : IRequest<Result<User, UserException>>
{
    public required Guid UserId { get; init; }
    public string RoleId { get; init; }
}

public class ChangeRoleForUserCommandHandler(
    IUserRepository userRepository, IRoleQueries roleQueries) : IRequestHandler<ChangeRoleForUserCommand, Result<User, UserException>>
{
    public async Task<Result<User, UserException>> Handle(
        ChangeRoleForUserCommand request,
        CancellationToken cancellationToken)
    {
        var userId = request.UserId;
        var existingUser = await userRepository.GetById(userId, cancellationToken);

        return await existingUser.Match(
            async user =>
            {
                var existingRole = await roleQueries.GetByName(request.RoleId, cancellationToken);

                return await existingRole.Match(
                    async role =>
                    {
                        try
                        {
                            var updateRolesModel = new UpdateRoleModel
                            {
                                UserId = userId,
                                RoleId = request.RoleId
                            };
                            var updatedUser = await userRepository.UpdateRole(updateRolesModel, cancellationToken);
                            return updatedUser;
                        }
                        catch (Exception exception)
                        {
                            return new UserUnknownException(user.Id, exception);
                        }
                    }, 
                    () => Task.FromResult<Result<User, UserException>>(new RoleNotFoundException(request.RoleId)));
                },
            () => Task.FromResult<Result<User, UserException>>(new UserNotFoundException(userId))
        );
    }
}