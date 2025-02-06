using Domain.Authentications.Users;

namespace Api.Dtos.Users;

public record UserDto(
    Guid? Id, 
    string Email,
    string? Name, 
    UserImageDto? Image,
    List<RoleDto>? Roles) 
{
    public static UserDto FromDomainModel(User user)
        => new(
            user.Id.Value,
            user.Email, 
            user.Name,
            user.UserImage != null ? UserImageDto.FromDomainModel(user.UserImage) : null,
            user.Roles.Select(RoleDto.FromDomainModel).ToList()); 
}
