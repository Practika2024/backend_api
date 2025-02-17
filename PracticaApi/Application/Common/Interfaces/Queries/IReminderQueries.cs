using Domain.Containers;
using Domain.Reminders;
using Optional;

namespace Application.Common.Interfaces.Queries
{
    public interface IReminderQueries
    {
        Task<IReadOnlyList<ReminderEntity>> GetAll(CancellationToken cancellationToken);
        Task<Option<ReminderEntity>> GetById(Guid id, CancellationToken cancellationToken);
        Task<IReadOnlyList<ReminderEntity>> GetByContainerId(Guid containerId, CancellationToken cancellationToken);
    }
}