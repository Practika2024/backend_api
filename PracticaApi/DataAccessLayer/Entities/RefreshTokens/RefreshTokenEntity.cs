using DataAccessLayer.Entities.Users;

namespace DataAccessLayer.Entities.RefreshTokens;

public class RefreshTokenEntity
{
    public Guid Id { get; set; }
    public string Token { get; set; }
    public string JwtId { get; set; }
    public bool IsUsed { get; set; } = false;
    public DateTime CreateDate { get; set; }
    public DateTime ExpiredDate { get; set; }
    public Guid UserId { get; set; }
    public UserEntity? User { get; set; }
}