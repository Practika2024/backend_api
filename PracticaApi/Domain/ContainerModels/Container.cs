using Domain.Common.Abstractions;
using Domain.ContainerHistoryModels;
using Domain.ContainerTypeModels;
using Domain.UserModels;

namespace Domain.ContainerModels;

public class Container : AuditableEntity<User>
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public decimal Volume { get; set; }
    public string? Notes { get; set; }
    public ICollection<ContainerHistory> Histories { get; set; } = new List<ContainerHistory>();
    public string UniqueCode { get; set; }
    public Guid TypeId { get; set; }
    public ContainerType? Type { get; set; }
    public Guid? ContentId { get; set; }
    public ContainerContent? Content { get; set; }
}