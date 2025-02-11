namespace Domain.Authentications.Users;

public class UserImageEntity
{
    public UserImageId Id { get; }
    public UserEntity UserEntity { get; }
    public UserId UserId { get; }
    public string FilePath { get; private set; }

    private UserImageEntity(UserImageId id, UserId userId, string filePath)
    {
        Id = id;
        UserId = userId;
        FilePath = filePath;
    }

    public static UserImageEntity New(UserImageId id, UserId userId, string filePath)
        => new(id, userId, filePath);
}