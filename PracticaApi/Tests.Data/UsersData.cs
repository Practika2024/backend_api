using Api.Dtos.Authentications;
using Domain.UserModels;

namespace Tests.Data;

public static class UsersData
{

    public static User MainUser = new User
    {
        Id = Guid.NewGuid(),

    };
}