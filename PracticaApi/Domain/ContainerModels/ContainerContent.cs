using Domain.Common.Abstractions;
using Domain.ProductModels;
using Domain.UserModels;

namespace Domain.ContainerModels;

public class ContainerContent : AuditableEntity<User>
{
    public Guid Id { get; set; }
    public Guid? ProductId { get; set; }
    public Product? Product { get; set; }
    public bool IsEmpty { get; set; }
}