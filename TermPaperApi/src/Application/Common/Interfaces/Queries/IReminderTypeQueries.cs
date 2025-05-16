using Domain.Reminders;
using Domain.ReminderTypes;
using Optional;

namespace Application.Common.Interfaces.Queries
{
    public interface IReminderTypeQueries
    {
        Task<IReadOnlyList<ReminderType>> GetAll(CancellationToken cancellationToken);
        Task<Option<ReminderType>> GetById(int id, CancellationToken cancellationToken);
    }
}