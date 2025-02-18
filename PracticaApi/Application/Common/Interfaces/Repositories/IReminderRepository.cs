using Application.Models.ReminderModels;
using Domain.Reminders;
using Optional;

namespace Application.Common.Interfaces.Repositories
{
    public interface IReminderRepository
    {
        Task<ReminderEntity> Create(CreateReminderModel model, CancellationToken cancellationToken);
        Task<ReminderEntity> Update(UpdateReminderModel model, CancellationToken cancellationToken);
        Task<ReminderEntity> Delete(DeleteReminderModel model, CancellationToken cancellationToken);
        Task<Option<ReminderEntity>> GetById(Guid id, CancellationToken cancellationToken);
    }
}