using Domain.ReminderModels;
using Optional;

namespace Application.Common.Interfaces.Repositories
{
    public interface IReminderRepository
    {
        Task<Reminder> Create(CreateReminderModel model, CancellationToken cancellationToken);
        Task<Reminder> Update(UpdateReminderModel model, CancellationToken cancellationToken);
        Task<Reminder> Delete(DeleteReminderModel model, CancellationToken cancellationToken);
        Task<Option<Reminder>> GetById(Guid id, CancellationToken cancellationToken);
    }
}