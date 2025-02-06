using Domain.Authentications.Users;

namespace Api.Dtos.Authentications;

public record SignInDto(string Email, string Password)
{
    public static SignInDto FromDomainModel(User user)
        => new(user.Email, user.PasswordHash);
}