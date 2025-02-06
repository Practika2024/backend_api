using Application.Common;
using Application.Exceptions;
using Application.ViewModels;

namespace Application.Services.UserServices.UpdateUserService;

public interface IUpdateUserService
{
    Task<Result<JwtVM, UserException>> UpdateUserAsync(Guid userId, string email, string userName, CancellationToken cancellationToken);
}