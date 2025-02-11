using Domain.Authentications.Users;
using Domain.Containers;
using Domain.Reminders;

namespace Application.Models.ReminderModels;
public class CreateReminderModel
{
    public ReminderId Id { get; set; }
    public ContainerId ContainerId { get; set; }
    public string Title { get; set; }
    public DateTime DueDate { get; set; }
    public ReminderType Type { get; set; }
    public UserId CreatedBy { get; set; }
}