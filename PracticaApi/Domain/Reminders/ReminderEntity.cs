using Domain.Abstractions;
using Domain.Containers;
using Domain.Users;

namespace Domain.Reminders;
internal class ReminderEntity : AuditableEntity
{
    public Guid Id { get; }
    public Guid ContainerId { get; set; }
    public ContainerEntity? Container { get; set; }
    public string Title { get; set; }
    public DateTime DueDate { get; set; }
    public ReminderType Type { get; set; }
}

public enum ReminderType
{
    Fermentation,
    Expiry
}