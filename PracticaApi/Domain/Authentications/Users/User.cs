using Domain.Authentications.Roles;
using Domain.ContainerHistories;
using Domain.Containers;
using Domain.Products;
using Domain.RefreshTokens;
using Domain.Reminders;

namespace Domain.Authentications.Users;

public class User
{
    public UserId Id { get; }
    public string Email { get; private set; }
    public string? Name { get; private set; }
    public string? Surname { get; private set; }
    public string? Patronymic { get; private set; }
    public string PasswordHash { get; }
    public UserImage? UserImage { get; private set; }
    public List<Role> Roles { get; private set; } = new();
    public List<RefreshToken> RefreshTokens { get; private set; } = new();

    public ICollection<Container> CreatedContainers { get; private set; } = new List<Container>();
    public ICollection<Product> CreatedProducts { get; private set; } = new List<Product>();
    public ICollection<ContainerHistory> CreatedHistories { get; private set; } = new List<ContainerHistory>();
    public ICollection<Reminder> CreatedReminders { get; private set; } = new List<Reminder>();

    private User(UserId id, string email, string? name, string? surname, string? patronymic, string passwordHash)
    {
        Id = id;
        Email = email;
        Name = name;
        Surname = surname;
        Patronymic = patronymic;
        PasswordHash = passwordHash;
    }

    public static User New(UserId id, string email, string? name, string? surname, string? patronymic,
        string passwordHash)
        => new(id, email, name, surname, patronymic, passwordHash);

    public void UpdateUser(string email, string? name)
    {
        Email = email;
        Name = name;
    }

    public void UpdateUserImage(UserImage userImage)
        => UserImage = userImage;

    public void SetRoles(List<Role> roles)
        => Roles = roles;
}