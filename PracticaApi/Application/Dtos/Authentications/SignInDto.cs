using Domain.Authentications.Users;

namespace Application.Dtos.Authentications;

public record SignInDto(string Email, string Password)
{
    public static SignInDto FromDomainModel(UserEntity userEntity)
        => new(userEntity.Email, userEntity.PasswordHash);
}