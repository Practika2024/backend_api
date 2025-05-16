namespace Domain.ReminderTypes.Models;

public class CreateReminderTypeModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public Guid CreatedBy { get; set; }
}