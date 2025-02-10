using Application.Common;
using Application.Exceptions;
using Domain.Authentications.Users;

namespace Application.Services.UserServices.ChangeRolesService;

public interface IChangeRolesService
{
    Task<Result<UserEntity, UserException>> ChangeRolesForUserAsync(Guid userId, List<string> roles, CancellationToken cancellationToken);
}