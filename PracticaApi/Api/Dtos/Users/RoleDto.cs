using Domain.Authentications.Roles;

namespace Api.Dtos.Users;

public record RoleDto(string Name)
{
    public static RoleDto FromDomainModel(Role role)
        => new(role.Name);
}