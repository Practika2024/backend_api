namespace Application.Services.ReminderService;

public interface IReminderService
{
    void ScheduleReminder(string userEmail, string title, DateTime reminderTime);
}