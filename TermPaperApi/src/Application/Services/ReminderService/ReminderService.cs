using Application.Services.EmailService;
using Hangfire;

namespace Application.Services.ReminderService;

public class ReminderService : IReminderService
{
    private readonly IEmailService _emailService;

    public ReminderService(IEmailService emailService)
    {
        _emailService = emailService;
    }

    public void ScheduleReminder(string userEmail, string title, DateTime reminderTime)
    {
        BackgroundJob.Schedule(() => SendReminder(userEmail, title), reminderTime - DateTime.Now);
    }

    public void SendReminder(string userEmail, string title)
    {
        var subject = $"Нагадування: {title}";
        var body = $"Шановний користувач, нагадуємо, що скоро настане подія: {title}. Будь ласка, підготуйтесь.";

        _emailService.SendEmail(userEmail, subject, body);
    }
}