using Domain.Authentications.Users;
using Domain.Containers;

namespace Domain.Reminders;
public class Reminder
{
    public ReminderId Id { get; }
    public ContainerId ContainerId { get; private set; }
    public Container? Container { get; private set; }
    public string Title { get; private set; }
    public DateTime DueDate { get; private set; }
    public ReminderType Type { get; private set; }
    public UserId CreatedBy { get; private set; }
    public User? CreatedByNavigation { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Reminder(
        ReminderId id,
        ContainerId containerId,
        string title,
        DateTime dueDate,
        ReminderType type,
        UserId createdBy)
    {
        Id = id;
        ContainerId = containerId;
        Title = title;
        DueDate = dueDate;
        Type = type;
        CreatedBy = createdBy;
        CreatedAt = DateTime.UtcNow;
    }

    public static Reminder New(
        ReminderId id,
        ContainerId containerId,
        string title,
        DateTime dueDate,
        ReminderType type,
        UserId createdBy)
        => new(id, containerId, title, dueDate, type, createdBy);

    public void Update(string title, DateTime dueDate, ReminderType type, UserId modifiedBy)
    {
        Title = title;
        DueDate = dueDate;
        Type = type;
    }
    

}

public record ReminderId(Guid Value)
{
    public static ReminderId New() => new(Guid.NewGuid());
    public static ReminderId Empty => new(Guid.Empty);
    public override string ToString() => Value.ToString();
}

public enum ReminderType
{
    Fermentation,
    Expiry
}