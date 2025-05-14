using Application.Services.EmailService;
using Hangfire;

namespace Application.Services.ReminderService;

public class ReminderService(IEmailService emailService) : IReminderService
{
    public string ScheduleReminder(string userEmail, string title, DateTime reminderTime)
    {
        var jobId = BackgroundJob.Schedule(() => SendReminder(userEmail, title), reminderTime - DateTime.UtcNow);
        return jobId;
    }

    public void DeleteHangfireJob(string reminderHangfireJobId)
    {
        BackgroundJob.Delete(reminderHangfireJobId);
    }

    public void SendReminder(string userEmail, string title)
    {
        var subject = $"Нагадування: {title}";
        var body = $"Шановний користувач, нагадуємо, що скоро настане подія: {title}. Будь ласка, підготуйтесь.";

        BackgroundJob.Enqueue(() => emailService.SendEmail(userEmail, subject, body));
    }
}