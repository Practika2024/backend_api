using FluentEmail.Core;

namespace Application.Services.EmailService;

public class EmailService : IEmailService
{
    private readonly IFluentEmail _fluentEmail;

    public EmailService(IFluentEmail fluentEmail)
    {
        _fluentEmail = fluentEmail;
    }

    public async Task SendEmail(string to, string subject, string body)
    {
        var result = await _fluentEmail
            .To(to)
            .Subject(subject)
            .Body(body, isHtml: true)
            .SendAsync();

        if (!result.Successful)
        {
            throw new Exception("Failed to send email.");
        }
    }
}