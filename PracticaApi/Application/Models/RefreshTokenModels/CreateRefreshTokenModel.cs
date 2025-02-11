using Domain.Authentications.Users;
using Domain.RefreshTokens;

namespace Application.Models.RefreshTokenModels;
public class CreateRefreshTokenModel
{
    public Guid Id { get; set; }
    public string Token { get; set; }
    public string JwtId { get; set; }
    public DateTime CreateDate { get; set; }
    public DateTime ExpiredDate { get; set; }
    public UserId UserId { get; set; }
}