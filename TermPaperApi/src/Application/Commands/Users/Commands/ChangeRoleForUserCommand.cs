using Application.Commands.Users.Exceptions;
using Application.Common;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Services;
using Domain.Users;
using Domain.Users.Models;
using MediatR;

namespace Application.Commands.Users.Commands;
public record ChangeRoleForUserCommand : IRequest<ServiceResponse>
{
    public required Guid UserId { get; init; }
    public string RoleId { get; init; }
}

public class ChangeRoleForUserCommandHandler(
    IUserRepository userRepository, IRoleQueries roleQueries) : IRequestHandler<ChangeRoleForUserCommand, ServiceResponse>
{
    public async Task<ServiceResponse> Handle(
        ChangeRoleForUserCommand request,
        CancellationToken cancellationToken)
    {
        var userId = request.UserId;
        var existingUser = await userRepository.GetById(userId, cancellationToken);

        return await existingUser.Match(
            async _ =>
            {
                var existingRole = await roleQueries.GetByName(request.RoleId, cancellationToken);

                return await existingRole.Match(
                    async _ =>
                    {
                        try
                        {
                            var updateRolesModel = new UpdateRoleModel
                            {
                                UserId = userId,
                                RoleId = request.RoleId
                            };
                            var updatedUser = await userRepository.UpdateRole(updateRolesModel, cancellationToken);
                            return ServiceResponse.OkResponse("User updated", updatedUser);
                        }
                        catch (Exception exception)
                        {
                            return ServiceResponse.InternalServerErrorResponse(exception.Message);
                        }
                    }, 
                    () => Task.FromResult(ServiceResponse.NotFoundResponse("Role not found")));
                },
            () => Task.FromResult(ServiceResponse.NotFoundResponse("User not found"))
        );
    }
}