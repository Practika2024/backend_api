using Domain.Authentications.Users;

namespace Application.Commands.Authentications.Services.TokenService
{
    public interface IJwtTokenService
    {
        string GenerateToken(UserEntity user);
    }
}
