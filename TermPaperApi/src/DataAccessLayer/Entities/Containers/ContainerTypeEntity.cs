﻿using DataAccessLayer.Entities.Users;
using Domain.Common.Abstractions;

namespace DataAccessLayer.Entities.Containers;

public class ContainerTypeEntity : AuditableEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; }
}