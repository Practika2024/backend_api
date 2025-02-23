using Domain.Common.Abstractions;
using Domain.UserModels;

namespace Domain.ContainerTypeModels;

public class ContainerType : AuditableEntity<User>
{
    public Guid Id { get; set; }
    public string Name { get; set; }
}