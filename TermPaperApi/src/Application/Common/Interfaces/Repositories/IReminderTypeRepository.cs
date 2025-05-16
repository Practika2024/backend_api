using Domain.ReminderTypes;
using Domain.ReminderTypes.Models;
using Optional;

namespace Application.Common.Interfaces.Repositories
{
    public interface IReminderTypeRepository
    {
        Task<ReminderType> Create(CreateReminderTypeModel model, CancellationToken cancellationToken);
        Task<ReminderType> Update(UpdateReminderTypeModel model, CancellationToken cancellationToken);
        Task<ReminderType> Delete(int id, CancellationToken cancellationToken);
        Task<Option<ReminderType>> GetById(int id, CancellationToken cancellationToken);
        Task<Option<ReminderType>> SearchByName(string name, CancellationToken cancellationToken);
    }
}