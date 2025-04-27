using Application.Services.EmailService;
using Hangfire;

namespace Application.Services.ReminderService;

public class ReminderService(IEmailService emailService) : IReminderService
{
    public void ScheduleReminder(string userEmail, string title, DateTime reminderTime)
    {
        BackgroundJob.Schedule(() => SendReminder(userEmail, title), reminderTime - DateTime.UtcNow);
    }

    public void SendReminder(string userEmail, string title)
    {
        var subject = $"Нагадування: {title}";
        var body = $"Шановний користувач, нагадуємо, що скоро настане подія: {title}. Будь ласка, підготуйтесь.";

        BackgroundJob.Enqueue(() => emailService.SendEmail(userEmail, subject, body));
    }
}