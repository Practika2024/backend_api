using Domain.Abstractions;

namespace Domain.Products;

internal class ProductTypeEntity : AuditableEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; }
}