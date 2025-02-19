using Domain.Abstractions;
using Domain.Containers;
using Domain.Users;

namespace Domain.Reminders;
public class ReminderEntity : AuditableEntity
{
    public Guid Id { get; }
    public Guid ContainerId { get; private set; }
    public ContainerEntity? Container { get; private set; }
    public string Title { get; private set; }
    public DateTime DueDate { get; private set; }
    public ReminderType Type { get; private set; }

    private ReminderEntity(
        Guid id,
        Guid containerId,
        string title,
        DateTime dueDate,
        ReminderType type,
        Guid createdBy) : base(createdBy)
    {
        Id = id;
        ContainerId = containerId;
        Title = title;
        DueDate = dueDate;
        Type = type;
    }

    public static ReminderEntity New(
        Guid id,
        Guid containerId,
        string title,
        DateTime dueDate,
        ReminderType type,
        Guid createdBy)
        => new(id, containerId, title, dueDate, type, createdBy);

    public void Update(string title, DateTime dueDate, ReminderType type, Guid modifiedBy)
    {
        Title = title;
        DueDate = dueDate;
        Type = type;
    }
    

}

public enum ReminderType
{
    Fermentation,
    Expiry
}