using Application.Common;
using Application.Exceptions;
using Application.ViewModels;

namespace Application.Services.AuthenticationServices.RefreshTokenService;

public interface IRefreshTokenService
{
    Task<Result<JwtVM, AuthenticationException>> RefreshTokenAsync(string accessToken, string refreshToken, CancellationToken cancellationToken);
}