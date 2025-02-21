using Domain.Abstractions;
using Domain.Products;

namespace Domain.Containers;

internal class ContainerContentEntity : AuditableEntity
{
    public Guid Id { get; set; }
    public Guid? ProductId { get; set; }
    public ProductEntity? Product { get; set; }
    public bool IsEmpty { get; set; }
}