using Domain.Authentications.Roles;

namespace Application.Dtos.Users;

public record RoleDto(string Name)
{
    public static RoleDto FromDomainModel(RoleEntity roleEntity)
        => new(roleEntity.Name);
}