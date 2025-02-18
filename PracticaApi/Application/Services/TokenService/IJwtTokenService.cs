using System.Security.Claims;
using Application.Models.UserModels;
using Domain.Users;

namespace Application.Services.TokenService
{
    public interface IJwtTokenService
    {
        Task<JwtModel> GenerateTokensAsync(UserEntity user, CancellationToken cancellationToken);
        ClaimsPrincipal GetPrincipals(string accessToken);
    }
}
