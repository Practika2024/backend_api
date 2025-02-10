using Domain.Reminders;
using Optional;

namespace Application.Common.Interfaces.Repositories;
public interface IReminderRepository
{
    Task<ReminderEntity> Create(ReminderEntity reminderEntity, CancellationToken cancellationToken);
    Task<ReminderEntity> Update(ReminderEntity reminderEntity, CancellationToken cancellationToken);
    Task<ReminderEntity> Delete(ReminderEntity reminderEntity, CancellationToken cancellationToken);
    Task<Option<ReminderEntity>> GetById(ReminderId id, CancellationToken cancellationToken);
}