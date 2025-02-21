using Domain.Users;

namespace Api.Dtos.Users;

public record UserDto(
    Guid? Id, 
    string Email,
    string? Name, 
    string? Surname, 
    string? Patronymic,
    string? Role) 
{
    public static UserDto FromDomainModel(UserEntity userEntity)
        => new(
            userEntity.Id,
            userEntity.Email, 
            userEntity.Name,
            userEntity.Surname,
            userEntity.Patronymic,
            userEntity.RoleId); 
}
