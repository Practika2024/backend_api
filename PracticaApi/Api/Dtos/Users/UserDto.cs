using Domain.Authentications.Users;

namespace Api.Dtos.Users;

public record UserDto(
    Guid? Id, 
    string Email,
    string? Name, 
    string? Surname, 
    string? Patronymic, 
    UserImageDto? Image,
    List<RoleDto>? Roles) 
{
    public static UserDto FromDomainModel(UserEntity userEntity)
        => new(
            userEntity.Id.Value,
            userEntity.Email, 
            userEntity.Name,
            userEntity.Surname,
            userEntity.Patronymic,
            userEntity.UserImage != null ? UserImageDto.FromDomainModel(userEntity.UserImage) : null,
            userEntity.Roles.Select(RoleDto.FromDomainModel).ToList()); 
}
