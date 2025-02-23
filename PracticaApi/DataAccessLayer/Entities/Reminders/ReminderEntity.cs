using DataAccessLayer.Entities.Containers;
using DataAccessLayer.Entities.Users;
using Domain.Common.Abstractions;
using Domain.ReminderModels;

namespace DataAccessLayer.Entities.Reminders;
public class ReminderEntity : AuditableEntity<UserEntity>
{
    public Guid Id { get; set; }
    public Guid ContainerId { get; set; }
    public ContainerEntity? Container { get; set; }
    public string Title { get; set; }
    public DateTime DueDate { get; set; }
    public ReminderType Type { get; set; }
}