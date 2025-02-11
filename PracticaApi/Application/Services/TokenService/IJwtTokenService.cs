using System.Security.Claims;
using Application.Models.UserModels;
using Domain.Authentications.Users;

namespace Application.Services.TokenService
{
    public interface IJwtTokenService
    {
        Task<JwtModel> GenerateTokensAsync(UserEntity userEntity, CancellationToken cancellationToken);
        ClaimsPrincipal GetPrincipals(string accessToken);
    }
}
