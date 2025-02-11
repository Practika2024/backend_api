using Domain.Authentications.Users;

namespace Domain.RefreshTokens
{
    public class RefreshTokenEntity
    {
        public Guid Id { get; }
        public string Token { get; private set; }
        public string JwtId { get; private set; }
        public bool IsUsed { get; set; } = false;
        public DateTime CreateDate { get; private set; }
        public DateTime ExpiredDate { get; private set; }
        public UserId UserId { get; private set; }
        public UserEntity? User { get; private set; }

        private RefreshTokenEntity(Guid id, string token, string jwtId, DateTime createDate, DateTime expiredDate,
            UserId userId)
        {
            Id = id;
            Token = token;
            JwtId = jwtId;
            CreateDate = createDate;
            ExpiredDate = expiredDate;
            UserId = userId;
        }

        public static RefreshTokenEntity New(Guid id, string token, string jwtId, DateTime createDate, DateTime expiredDate,
            UserId userId)
            => new(id, token, jwtId, createDate, expiredDate, userId);
    }
}