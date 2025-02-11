using Domain.Authentications.Users;

namespace Application.Dtos.Users;

public record UpdateUserDto(
    string Email,
    string? Name, 
    string? Surname, 
    string? Patronymic) 
{
    public static UpdateUserDto FromDomainModel(UserEntity userEntity)
        => new(
            userEntity.Email, 
            userEntity.Name,
            userEntity.Surname,
            userEntity.Patronymic); 
}