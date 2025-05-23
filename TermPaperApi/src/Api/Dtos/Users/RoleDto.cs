﻿using DataAccessLayer.Entities.Roles;

namespace Api.Dtos.Users;

public record RoleDto(string Name)
{
    public static RoleDto FromDomainModel(RoleEntity roleEntity)
        => new(roleEntity.Name);
}