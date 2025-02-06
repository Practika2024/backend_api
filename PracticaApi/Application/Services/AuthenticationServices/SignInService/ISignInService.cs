using Application.Common;
using Application.Exceptions;
using Application.ViewModels;

namespace Application.Services.AuthenticationServices.SignInService;

public interface ISignInService
{
    Task<Result<JwtVM, AuthenticationException>> SignInAsync(string email, string password, CancellationToken cancellationToken);
}