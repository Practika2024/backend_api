using System.Security.Claims;
using Domain.UserModels;

namespace Application.Services.TokenService
{
    public interface IJwtTokenService
    {
        Task<JwtModel> GenerateTokensAsync(User user, CancellationToken cancellationToken);
        ClaimsPrincipal GetPrincipals(string accessToken);
    }
}
