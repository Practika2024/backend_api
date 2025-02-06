namespace Domain.Authentications.Users;

public class UserImage
{
    public UserImageId Id { get; }
    public User User { get; }
    public UserId UserId { get; }
    public string FilePath { get; private set; }

    private UserImage(UserImageId id, UserId userId, string filePath)
    {
        Id = id;
        UserId = userId;
        FilePath = filePath;
    }

    public static UserImage New(UserImageId id, UserId userId, string filePath)
        => new(id, userId, filePath);
}