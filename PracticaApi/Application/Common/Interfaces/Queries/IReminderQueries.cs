using Domain.Reminders;
using Optional;
using Domain.Containers;

namespace Application.Common.Interfaces.Queries;
public interface IReminderQueries
{
    Task<IReadOnlyList<ReminderEntity>> GetAll(CancellationToken cancellationToken);
    Task<Option<ReminderEntity>> GetById(ReminderId id, CancellationToken cancellationToken);
    Task<IReadOnlyList<ReminderEntity>> GetByContainerId(ContainerId containerId, CancellationToken cancellationToken);
}