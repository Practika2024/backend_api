using Domain.ContainerHistories;
using Domain.Containers;
using Domain.Products;
using Domain.RefreshTokens;
using Domain.Reminders;
using Domain.Roles;

namespace Domain.Users;

public class UserEntity
{
    public Guid Id { get; }
    public string Email { get; private set; }
    public string? Name { get; private set; }
    public string? Surname { get; private set; }
    public string? Patronymic { get; private set; }
    public string PasswordHash { get; }
    public string RoleId { get; private set; }
    public RoleEntity? Role { get; private set; }
    public List<RefreshTokenEntity> RefreshTokens { get; private set; } = new();

    private UserEntity(Guid id, string roleId, string email, string? name, string? surname, string? patronymic,
        string passwordHash)
    {
        Id = id;
        RoleId = roleId;
        Email = email;
        Name = name;
        Surname = surname;
        Patronymic = patronymic;
        PasswordHash = passwordHash;
    }

    public static UserEntity New(Guid id, string roleId, string email, string? name, string? surname,
        string? patronymic,
        string passwordHash)
        => new(id, roleId, email, name, surname, patronymic, passwordHash);

    public void UpdateUser(string email, string? name, string? surname, string? patronymic)
    {
        Email = email;
        Name = name;
        Surname = surname;
        Patronymic = patronymic;
    }
}