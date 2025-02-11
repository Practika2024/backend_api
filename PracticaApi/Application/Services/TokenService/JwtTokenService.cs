using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Application.Common.Interfaces.Repositories;
using Application.Models.RefreshTokenModels;
using Application.Models.UserModels;
using Domain.Authentications;
using Domain.Authentications.Users;
using Domain.RefreshTokens;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Application.Services.TokenService
{
    public class JwtTokenService(IConfiguration configuration, IRefreshTokenRepository refreshTokenRepository) : IJwtTokenService
    {
        private JwtSecurityToken GenerateAccessToken(UserEntity userEntity)
        {
            var issuer = configuration["AuthSettings:issuer"];
            var audience = configuration["AuthSettings:audience"];
            var keyString = configuration["AuthSettings:key"];
            var symmetricKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString!));

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("id", userEntity.Id.Value.ToString()),
                new Claim("email", userEntity.Email!),
                new Claim("name", userEntity.Name ?? "N/A"),
                new Claim("image", userEntity.UserImage?.FilePath ?? "N/A"),
            };

            if (userEntity.Roles.Count() > 0)
            {
                var roleClaims = userEntity.Roles.Select(ur => new Claim(
                    "role",
                    ur.Name
                )).ToArray();

                claims.AddRange(roleClaims);
            }
            else
            {
                claims.Add(new Claim("role", AuthSettings.OperatorRole));
            }
            
            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: new SigningCredentials(symmetricKey, SecurityAlgorithms.HmacSha256)
            );

            return token;
        }
        
        private string GenerateRefreshToken()
        {
            var bytes = new byte[32];

            using (var rnd = RandomNumberGenerator.Create())
            {
                rnd.GetBytes(bytes);
                return Convert.ToBase64String(bytes);
            }
        }

        private async Task<RefreshTokenEntity?> SaveRefreshTokenAsync(UserEntity userEntity, string refreshToken, string jwtId, CancellationToken cancellationToken)
        {
            var model = new CreateRefreshTokenModel
            {
                Id = Guid.NewGuid(),
                Token = refreshToken,
                JwtId = jwtId,
                CreateDate = DateTime.UtcNow,
                ExpiredDate = DateTime.UtcNow.AddDays(7),
                UserId = userEntity.Id
            };

            try
            {
                var tokenEntity = await refreshTokenRepository.Create(model, cancellationToken);
                return tokenEntity;
            }
            catch (Exception e)
            {
                return null;
            }
        }
        
        public ClaimsPrincipal GetPrincipals(string accessToken)
        {
            var jwtSecurityKey = configuration["AuthSettings:key"];

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecurityKey))
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var principals = tokenHandler.ValidateToken(accessToken, validationParameters, out SecurityToken securityToken);

            var jwtSecurityToken = securityToken as JwtSecurityToken;

            if(jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256))
            {
                throw new SecurityTokenException("Invalid access token");
            }

            return principals;
        }
        
        public async Task<JwtModel> GenerateTokensAsync(UserEntity userEntity, CancellationToken cancellationToken)
        {
            var accessToken = GenerateAccessToken(userEntity);
            var refreshToken = GenerateRefreshToken();

            await refreshTokenRepository.MakeAllRefreshTokensExpiredForUser(userEntity.Id, cancellationToken);
            
            var saveResult = await SaveRefreshTokenAsync(userEntity, refreshToken, accessToken.Id, cancellationToken);

            var tokens = new JwtModel
            {
                AccessToken = new JwtSecurityTokenHandler().WriteToken(accessToken),
                RefreshToken = refreshToken
            };

            return tokens;
        }
    }
}
