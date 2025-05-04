using Domain.Common.Abstractions;
using Domain.Users;

namespace Domain.ProductTypes;

public class ProductType : AuditableEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; }
}