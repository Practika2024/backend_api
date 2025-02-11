using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Domain.Authentications;
using Domain.Authentications.Users;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Application.Commands.Authentications.Services.TokenService
{
    public class JwtTokenService : IJwtTokenService
    {
        private readonly IConfiguration _configuration;

        public JwtTokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateToken(UserEntity user)
        {
            var issuer = _configuration["AuthSettings:issuer"];
            var audience = _configuration["AuthSettings:audience"];
            var keyString = _configuration["AuthSettings:key"];
            var symmetricKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString!));

            var claims = new List<Claim>
            {
                new Claim("id", user.Id.Value.ToString()),
                new Claim("email", user.Email!),
                new Claim("name", user.Name ?? "N/A")
            };

            if (user.Roles.Count() > 0)
            {
                var roleClaims = user.Roles.Select(ur => new Claim(
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

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
