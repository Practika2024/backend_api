using Application.ViewModels;
using Domain.Reminders;

namespace Api.Dtos.Reminders;
public class ReminderDto
{
    public Guid Id { get; set; }
    public Guid ContainerId { get; set; }
    public string Title { get; set; }
    public DateTime DueDate { get; set; }
    public ReminderType Type { get; set; }

    public static ReminderDto FromDomainModel(Reminder reminder)
    {
        return new ReminderDto
        {
            Id = reminder.Id.Value,
            ContainerId = reminder.ContainerId.Value,
            Title = reminder.Title,
            DueDate = reminder.DueDate,
            Type = reminder.Type
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