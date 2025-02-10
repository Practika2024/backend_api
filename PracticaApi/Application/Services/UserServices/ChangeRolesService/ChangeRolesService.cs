using Application.Common;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Exceptions;
using Domain.Authentications.Roles;
using Domain.Authentications.Users;

namespace Application.Services.UserServices.ChangeRolesService;

public class ChangeRolesService : IChangeRolesService
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleQueries _roleQueries;

    public ChangeRolesService(IUserRepository userRepository, IRoleQueries roleQueries)
    {
        _userRepository = userRepository;
        _roleQueries = roleQueries;
    }

    public async Task<Result<UserEntity, UserException>> ChangeRolesForUserAsync(
        Guid userId,
        List<string> roles,
        CancellationToken cancellationToken)
    {
        var userIdObj = new UserId(userId);
        var existingUser = await _userRepository.GetById(userIdObj, cancellationToken);

        var rolesList = new List<RoleEntity>();
        foreach (var role in roles)
        {
            var existingRole = await _roleQueries.GetByName(role, cancellationToken);

            var roleResult = await existingRole.Match<Task<Result<RoleEntity, UserException>>>(
                async r =>
                {
                    rolesList.Add(r);
                    return r;
                },
                () => Task.FromResult<Result<RoleEntity, UserException>>(new RoleNotFoundException(role))
            );

            if (roleResult.IsError)
            {
                return new RoleNotFoundException(role);
            }
        }

        return await existingUser.Match<Task<Result<UserEntity, UserException>>>(
            async user => await ChangeRolesForUser(user, rolesList, cancellationToken),
            () => Task.FromResult<Result<UserEntity, UserException>>(new UserNotFoundException(userIdObj))
        );
    }

    private async Task<Result<UserEntity, UserException>> ChangeRolesForUser(
        UserEntity userEntity,
        List<RoleEntity> roles,
        CancellationToken cancellationToken)
    {
        try
        {
            userEntity.SetRoles(roles);
            return await _userRepository.Update(userEntity, cancellationToken);
        }
        catch (Exception exception)
        {
            return new UserUnknownException(userEntity.Id, exception);
        }
    }
}