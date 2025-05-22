namespace Domain.Reminders.Models;
public class UpdateReminderModel
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public DateTime DueDate { get; set; }
    public ReminderType Type { get; set; }
    public Guid ModifiedBy { get; set; }
    public Guid ContainerId { get; set; }
    public string HangfireJobId { get; set; }
}