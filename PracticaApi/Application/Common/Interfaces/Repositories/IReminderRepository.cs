using Domain.Reminders;
using Optional;

namespace Application.Common.Interfaces.Repositories;
public interface IReminderRepository
{
    Task<Reminder> Create(Reminder reminder, CancellationToken cancellationToken);
    Task<Reminder> Update(Reminder reminder, CancellationToken cancellationToken);
    Task<Reminder> Delete(Reminder reminder, CancellationToken cancellationToken);
    Task<Option<Reminder>> GetById(ReminderId id, CancellationToken cancellationToken);
}