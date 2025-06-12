using Domain.Reminders;

namespace Tests.Data;

public class ReminderData
{
    public static Reminder MainReminder(int reminderTypeId, Guid containerId) => new()
    {
        Id = Guid.NewGuid(),
        ContainerId = containerId,
        Title = "Test reminder",
        DueDate = DateTime.UtcNow.AddDays(1),
        TypeId = reminderTypeId,
        IsViewed = false,
    };
}