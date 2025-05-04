namespace Application.Services.EmailService;

public interface IEmailService
{
    Task SendEmail(string to, string subject, string body);
}