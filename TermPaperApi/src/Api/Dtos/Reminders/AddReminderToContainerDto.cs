namespace Api.Dtos.Reminders;

public class AddReminderToContainerDto
{
    public string Title { get; set; } = null!;
    public DateTime DueDate { get; set; }
    public int Type { get; set; }
}