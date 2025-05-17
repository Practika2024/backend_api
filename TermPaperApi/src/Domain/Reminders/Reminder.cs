using Domain.Common.Abstractions;
using Domain.Containers;
using Domain.ProductTypes;
using Domain.ReminderTypes;
using Domain.Users;

namespace Domain.Reminders;
public class Reminder : AuditableEntity
{
    public Guid Id { get; set; }
    public Guid ContainerId { get; set; }
    public Container? Container { get; set; }
    public string Title { get; set; }
    public DateTime DueDate { get; set; }
    public int TypeId { get; set; }
    public ReminderType? Type { get; set; }
    public string? HangfireJobId { get; set; }
    public ReminderStatus Status { get; set; }
}

public enum ReminderStatus
{
    Active,
    Completed,
    Viewed,
}