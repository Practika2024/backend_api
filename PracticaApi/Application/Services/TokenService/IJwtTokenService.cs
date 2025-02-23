using System.Security.Claims;
using Domain.UserModels;
using Google.Apis.Auth;

namespace Application.Services.TokenService
{
    public interface IJwtTokenService
    {
        Task<JwtModel> GenerateTokensAsync(User user, CancellationToken cancellationToken);
        ClaimsPrincipal GetPrincipals(string accessToken);
        Task<GoogleJsonWebSignature.Payload> VerifyGoogleToken(ExternalLoginModel model);
    }
}
