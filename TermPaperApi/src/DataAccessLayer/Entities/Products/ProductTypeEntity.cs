﻿using DataAccessLayer.Entities.Users;
using Domain.Common.Abstractions;

namespace DataAccessLayer.Entities.Products;

public class ProductTypeEntity : AuditableEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; }
}