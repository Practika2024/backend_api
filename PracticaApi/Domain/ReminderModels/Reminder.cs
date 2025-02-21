using Domain.Common.Abstractions;
using Domain.ContainerModels;
using Domain.UserModels;

namespace Domain.ReminderModels;
public class Reminder : AuditableEntity<User>
{
    public Guid Id { get; set; }
    public Guid ContainerId { get; set; }
    public Container? Container { get; set; }
    public string Title { get; set; }
    public DateTime DueDate { get; set; }
    public ReminderType Type { get; set; }
}

public enum ReminderType
{
    Fermentation,
    Expiry
}