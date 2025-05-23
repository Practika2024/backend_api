using Domain.Reminders;
using Optional;

namespace Application.Common.Interfaces.Queries
{
    public interface IReminderQueries
    {
        Task<IReadOnlyList<Reminder>> GetAll(CancellationToken cancellationToken);
        Task<Option<Reminder>> GetById(Guid id, CancellationToken cancellationToken);
        Task<Option<IReadOnlyList<Reminder>>> GetAllByUser(Guid userId, CancellationToken cancellationToken);
        Task<Option<IReadOnlyList<Reminder>>> GetAllCompletedByUser(Guid userId, CancellationToken cancellationToken);
        Task<IReadOnlyList<Reminder>> GetByContainerId(Guid containerId, CancellationToken cancellationToken);
    }
}