using Application.ViewModels;
using Domain.Reminders;

namespace Application.Dtos.Reminders;
public class ReminderDto
{
    public Guid Id { get; set; }
    public Guid ContainerId { get; set; }
    public string Title { get; set; }
    public DateTime DueDate { get; set; }
    public ReminderType Type { get; set; }

    public static ReminderDto FromDomainModel(ReminderEntity reminderEntity)
    {
        return new ReminderDto
        {
            Id = reminderEntity.Id.Value,
            ContainerId = reminderEntity.ContainerId.Value,
            Title = reminderEntity.Title,
            DueDate = reminderEntity.DueDate,
            Type = reminderEntity.Type
        };
    }
    public static ReminderDto FromDomainModel(ReminderVM reminder)
    {
        return new ReminderDto
        {
            Id = reminder.Id,
            ContainerId = reminder.ContainerId,
            Title = reminder.Title,
            DueDate = reminder.DueDate,
            Type = reminder.Type
        };
    }
}