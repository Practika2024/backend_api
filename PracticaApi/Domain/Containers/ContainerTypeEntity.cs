using Domain.Abstractions;

namespace Domain.Containers;

internal class ContainerTypeEntity : AuditableEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; }
}