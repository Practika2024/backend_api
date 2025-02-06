using Application.Common;
using Application.Exceptions;
using Application.ViewModels;

namespace Application.Services.AuthenticationServices.SignUpService;

public interface ISignUpService
{
    Task<Result<JwtVM, AuthenticationException>> SignUpAsync(string email, string password, string name, string surname,
        string patronymic, CancellationToken cancellationToken);
}