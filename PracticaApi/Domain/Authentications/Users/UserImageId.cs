namespace Domain.Authentications.Users;

public record UserImageId(Guid Value)
{
    public static UserImageId New() => new(Guid.NewGuid());
    public static UserImageId Empty => new(Guid.Empty);
    public override string ToString() => Value.ToString();
}