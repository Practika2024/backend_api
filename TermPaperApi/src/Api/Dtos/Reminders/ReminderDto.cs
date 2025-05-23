namespace Api.Dtos.Reminders;

public class ReminderDto
{
    public Guid Id { get; set; }
    public Guid ContainerId { get; set; }
    public string Title { get; set; }
    public DateTime DueDate { get; set; }
    public int TypeId { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsViewed { get; set; }
}