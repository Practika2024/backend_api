﻿using Domain.Common.Abstractions;
using Domain.Users;

namespace Domain.ContainerTypes;

public class ContainerType : AuditableEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; }
}