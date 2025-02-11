using Domain.Reminders;

namespace Application.Dtos.Reminders;
public class ReminderDto
{
    public Guid Id { get; set; }
    public Guid ContainerId { get; set; }
    public string Title { get; set; }
    public DateTime DueDate { get; set; }
    public ReminderType Type { get; set; }
    public Guid CreatedBy { get; private set; }
    
    public DateTime CreatedAt { get; private set; }



    public static ReminderDto FromDomainModel(ReminderEntity reminderEntity)
    {
        return new ReminderDto
        {
            Id = reminderEntity.Id.Value,
            ContainerId = reminderEntity.ContainerId.Value,
            Title = reminderEntity.Title,
            DueDate = reminderEntity.DueDate,
            Type = reminderEntity.Type,
            CreatedAt = reminderEntity.CreatedAt,
            CreatedBy = reminderEntity.CreatedBy.Value
        };
    }
}