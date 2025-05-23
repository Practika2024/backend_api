using Domain.Common.Abstractions;

namespace DataAccessLayer.Entities.Reminders;

public class ReminderTypeEntity : AuditableEntity
{
    public int Id { get; set; }
    public string Name { get; set; }
}