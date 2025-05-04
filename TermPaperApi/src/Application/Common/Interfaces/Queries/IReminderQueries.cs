using Domain.Reminders;
using Optional;

namespace Application.Common.Interfaces.Queries
{
    public interface IReminderQueries
    {
        Task<IReadOnlyList<Reminder>> GetAll(CancellationToken cancellationToken);
        Task<Option<Reminder>> GetById(Guid id, CancellationToken cancellationToken);
        Task<IReadOnlyList<Reminder>> GetByContainerId(Guid containerId, CancellationToken cancellationToken);
    }
}