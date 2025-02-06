using System.Security.Claims;
using Application.ViewModels;
using Domain.Authentications.Users;

namespace Application.Services.TokenService
{
    public interface IJwtTokenService
    {
        Task<JwtVM> GenerateTokensAsync(User user, CancellationToken cancellationToken);
        ClaimsPrincipal GetPrincipals(string accessToken);
    }
}
