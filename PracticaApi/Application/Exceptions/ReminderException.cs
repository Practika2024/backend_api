using Domain.Reminders;

namespace Application.Exceptions;
public abstract class ReminderException(ReminderId id, string message, Exception innerException = null) : Exception(message, innerException)
{
    public ReminderId ReminderId { get; } = id;
}

public class ReminderNotFoundException(ReminderId id) : ReminderException(id, $"Reminder not found! ID: {id}");