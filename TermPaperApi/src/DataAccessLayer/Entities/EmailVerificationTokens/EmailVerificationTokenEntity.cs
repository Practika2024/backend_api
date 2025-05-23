namespace DataAccessLayer.Entities.EmailVerificationTokens;

public class EmailVerificationTokenEntity
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public DateTime CreatedOnUtc { get; set; }
    public DateTime ExpiresOnUtc { get; set; }
}