using Domain.Common.Abstractions;
using Domain.UserModels;

namespace Domain.ProductModels;

public class ProductType : AuditableEntity<User>
{
    public Guid Id { get; set; }
    public string Name { get; set; }
}