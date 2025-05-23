﻿using DataAccessLayer.Entities.ContainerHistories;
using DataAccessLayer.Entities.Products;
using DataAccessLayer.Entities.Users;
using Domain.Common.Abstractions;

namespace DataAccessLayer.Entities.Containers;

public class ContainerEntity : AuditableEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public decimal Volume { get; set; }
    public string? Notes { get; set; }
    public string UniqueCode { get; set; }
    public Guid TypeId { get; set; }
    public ContainerTypeEntity? Type { get; set; }
    public Guid? ProductId { get; set; }
    public ProductEntity? Product { get; set; }
    public string? FilePath { get; set; }
}