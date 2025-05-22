namespace Application.Services.ReminderService;

public interface IReminderService
{
    string ScheduleReminder(string userEmail, string title, DateTime reminderTime);
    void DeleteHangfireJob(string reminderHangfireJobId);
}