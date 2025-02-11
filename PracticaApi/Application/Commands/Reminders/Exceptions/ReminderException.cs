using Domain.Reminders;

namespace Application.Commands.Reminders.Exceptions;
public abstract class ReminderException(ReminderId id, string message, Exception innerException = null) : Exception(message, innerException)
{
    public ReminderId ReminderId { get; } = id;
}

public class ReminderNotFoundException(ReminderId id) : ReminderException(id, $"Reminder not found! ID: {id}");
public class ContainerForReminderNotFoundException(ReminderId id) : ReminderException(id, $"Container not found! Reminder ID: {id}");

public class ReminderUnknownException(ReminderId id, ReminderException innerException)
    : ReminderException(id, $"Unknown exception for the Reminder under id: {id}", innerException);