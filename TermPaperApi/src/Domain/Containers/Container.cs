using Domain.Common.Abstractions;
using Domain.ContainerTypes;
using Domain.Products;
using Domain.Users;

namespace Domain.Containers;

public class Container : AuditableEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public decimal Volume { get; set; }
    public string? Notes { get; set; }
    public string UniqueCode { get; set; }
    public Guid TypeId { get; set; }
    public ContainerType? Type { get; set; }
    public Guid? ProductId { get; set; }
    public Product? Product { get; set; }
}