using Domain.Containers;
using Domain.Interfaces;
using Domain.Users;

namespace Domain.Reminders;
public class ReminderEntity : IAuditableEntity
{
    public Guid Id { get; }
    public Guid ContainerId { get; private set; }
    public ContainerEntity? Container { get; private set; }
    public string Title { get; private set; }
    public DateTime DueDate { get; private set; }
    public ReminderType Type { get; private set; }
    public Guid CreatedBy { get; private set; }
    public UserEntity? CreatedByNavigation { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public Guid ModifiedBy { get; }
    public DateTime ModifiedAt { get; }

    private ReminderEntity(
        Guid id,
        Guid containerId,
        string title,
        DateTime dueDate,
        ReminderType type,
        Guid createdBy)
    {
        Id = id;
        ContainerId = containerId;
        Title = title;
        DueDate = dueDate;
        Type = type;
        CreatedBy = createdBy;
        CreatedAt = DateTime.UtcNow;
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