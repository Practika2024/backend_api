namespace Application.Commands.Reminders.Exceptions;

public abstract class ReminderException(Guid id, string message, Exception innerException = null)
    : Exception(message, innerException)
{
    public Guid ReminderId { get; } = id;
}

public class ReminderNotFoundException(Guid id) : ReminderException(id, $"Reminder not found! ID: {id}");

public class ContainerForReminderNotFoundException(Guid id)
    : ReminderException(id, $"Container not found! Reminder ID: {id}");

public class ReminderUnknownException(Guid id, ReminderException innerException)
    : ReminderException(id, $"Unknown exception for the Reminder under id: {id}", innerException);