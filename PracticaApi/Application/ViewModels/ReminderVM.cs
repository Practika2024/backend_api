using Domain.Reminders;

namespace Application.ViewModels;
public class ReminderVM
{
    public Guid Id { get; set; }
    public Guid ContainerId { get; set; }
    public string Title { get; set; }
    public DateTime DueDate { get; set; }
    public ReminderType Type { get; set; }

    public ReminderVM(ReminderEntity reminderEntity)
    {
        Id = reminderEntity.Id.Value;
        ContainerId = reminderEntity.ContainerId.Value;
        Title = reminderEntity.Title;
        DueDate = reminderEntity.DueDate;
        Type = reminderEntity.Type;
    }
}