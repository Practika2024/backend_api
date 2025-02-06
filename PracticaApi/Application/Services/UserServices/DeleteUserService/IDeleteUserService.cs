using Application.Common;
using Application.Exceptions;
using Domain.Authentications.Users;

namespace Application.Services.UserServices.DeleteUserService;


public interface IDeleteUserService
{
    Task<Result<User, UserException>> DeleteUserAsync(Guid userId, CancellationToken cancellationToken);
}