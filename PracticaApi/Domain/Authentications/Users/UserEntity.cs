using Domain.Authentications.Roles;
using Domain.ContainerHistories;
using Domain.Containers;
using Domain.Products;
using Domain.RefreshTokens;
using Domain.Reminders;

namespace Domain.Authentications.Users;

public class UserEntity
{
    public UserId Id { get; }
    public string Email { get; private set; }
    public string? Name { get; private set; }
    public string? Surname { get; private set; }
    public string? Patronymic { get; private set; }
    public string PasswordHash { get; }
    public UserImageEntity? UserImage { get; private set; }
    public List<RoleEntity> Roles { get; private set; } = new();
    public List<RefreshTokenEntity> RefreshTokens { get; private set; } = new();

    public ICollection<ContainerEntity> CreatedContainers { get; private set; } = new List<ContainerEntity>();
    public ICollection<ProductEntity> CreatedProducts { get; private set; } = new List<ProductEntity>();
    public ICollection<ContainerHistoryEntity> CreatedHistories { get; private set; } = new List<ContainerHistoryEntity>();
    public ICollection<ReminderEntity> CreatedReminders { get; private set; } = new List<ReminderEntity>();

    private UserEntity(UserId id, string email, string? name, string? surname, string? patronymic, string passwordHash)
    {
        Id = id;
        Email = email;
        Name = name;
        Surname = surname;
        Patronymic = patronymic;
        PasswordHash = passwordHash;
    }

    public static UserEntity New(UserId id, string email, string? name, string? surname, string? patronymic,
        string passwordHash)
        => new(id, email, name, surname, patronymic, passwordHash);

    public void UpdateUser(string email, string? name)
    {
        Email = email;
        Name = name;
    }

    public void UpdateUserImage(UserImageEntity userImageEntity)
        => UserImage = userImageEntity;

    public void SetRoles(List<RoleEntity> roles)
        => Roles = roles;
}