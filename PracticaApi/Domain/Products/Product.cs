using Domain.Common.Abstractions;
using Domain.ContainersHistory;
using Domain.ProductTypes;
using Domain.Users;

namespace Domain.Products;
public class Product : AuditableEntity<User>
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public DateTime ManufactureDate { get; set; }
    public Guid TypeId { get; set; }
    public ProductType? Type { get; set; }
    public ICollection<ContainerHistory> Histories { get; set; } = new List<ContainerHistory>();
}