using Domain.Reminders;

namespace Application.ViewModels;
public class ReminderVM
{
    public Guid Id { get; set; }
    public Guid ContainerId { get; set; }
    public string Title { get; set; }
    public DateTime DueDate { get; set; }
    public ReminderType Type { get; set; }

    public ReminderVM(Reminder reminder)
    {
        Id = reminder.Id.Value;
        ContainerId = reminder.ContainerId.Value;
        Title = reminder.Title;
        DueDate = reminder.DueDate;
        Type = reminder.Type;
    }
}