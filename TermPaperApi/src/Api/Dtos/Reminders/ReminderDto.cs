using Domain.Reminders;

namespace Api.Dtos.Reminders;
public class ReminderDto
{
    public Guid Id { get; set; }
    public Guid ContainerId { get; set; }
    public string Title { get; set; }
    public DateTime DueDate { get; set; }
    public int TypeId { get; set; }
    public Guid CreatedBy { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public string Status { get; set; }
}