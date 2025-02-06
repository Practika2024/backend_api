using Domain.Reminders;
using Optional;
using Domain.Containers;

namespace Application.Common.Interfaces.Queries;
public interface IReminderQueries
{
    Task<IReadOnlyList<Reminder>> GetAll(CancellationToken cancellationToken);
    Task<Option<Reminder>> GetById(ReminderId id, CancellationToken cancellationToken);
    Task<IReadOnlyList<Reminder>> GetByContainerId(ContainerId containerId, CancellationToken cancellationToken);
}