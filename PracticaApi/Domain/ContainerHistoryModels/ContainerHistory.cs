using Domain.Common.Abstractions;
using Domain.ContainerModels;
using Domain.ProductModels;
using Domain.UserModels;

namespace Domain.ContainerHistoryModels;

public class ContainerHistory : AuditableEntity<User>
{
    public Guid Id { get; set; }
    public Guid ContainerId { get; set; }
    public Container? Container { get; set; }
    public Guid? ProductId { get; set; }
    public Product? Product { get; set; }
    public DateTime StartDate { get; }
    public DateTime EndDate { get; set; }
}