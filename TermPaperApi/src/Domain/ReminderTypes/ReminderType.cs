using Domain.Common.Abstractions;

namespace Domain.ReminderTypes;

public class ReminderType : AuditableEntity
{
    public int Id { get; set; }
    public string Name { get; set; }
}