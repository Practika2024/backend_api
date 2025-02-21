namespace Domain.ReminderModels;
public class CreateReminderModel
{
    public Guid Id { get; set; }
    public Guid ContainerId { get; set; }
    public string Title { get; set; }
    public DateTime DueDate { get; set; }
    public ReminderType Type { get; set; }
    public Guid CreatedBy { get; set; }
}