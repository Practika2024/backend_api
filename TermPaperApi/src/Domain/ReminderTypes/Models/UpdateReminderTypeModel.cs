namespace Domain.ReminderTypes.Models;

public class UpdateReminderTypeModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public Guid ModifiedBy { get; set; }
}