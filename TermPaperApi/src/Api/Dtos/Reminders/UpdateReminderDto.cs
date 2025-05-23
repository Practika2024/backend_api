using Domain.Reminders;

namespace Api.Dtos.Reminders;

public class UpdateReminderDto
{
    public string? Title { get; set; }
    public DateTime? DueDate { get; set; }
    public int? Type { get; set; }
    public Guid? ContainerId { get; set; }
}