using Domain.Authentications.Users;

namespace Application.Dtos.Authentications;

public record SignUpDto(string Email, string Password, string? Name, string? Surname, string? Patronymic)
{
    public static SignUpDto FromDomainModel(UserEntity userEntity)
        => new(userEntity.Email, userEntity.PasswordHash, userEntity.Name, userEntity.Surname, userEntity.Patronymic);
}