using Api.Dtos.Authentications;
using Domain.Users;

namespace Tests.Data;

public static class UsersData
{
    public static User MainUser() => new()
    {
        Id = Guid.NewGuid(),
        Email = "test@example.com",
        Name = "Test",
        Surname = "User",
        Patronymic = "Testovich",
        RoleId = "Admin"
    };
}