using Domain.Reminders;

namespace Api.ViewModels.Reminders;
public class CreateReminderVM
{
    public required string Title { get; init; }
    public required DateTime DueDate { get; init; }
    public required ReminderType Type { get; init; }
}