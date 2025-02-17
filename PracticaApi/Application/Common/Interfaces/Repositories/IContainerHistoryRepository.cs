using Application.Models.ContainerHistoryModels;
using Domain.ContainerHistories;
using Optional;

namespace Application.Common.Interfaces.Repositories
{
    public interface IContainerHistoryRepository
    {
        Task<ContainerHistoryEntity> Create(CreateContainerHistoryModel model, CancellationToken cancellationToken);
        Task<ContainerHistoryEntity> Update(UpdateContainerHistoryModel model, CancellationToken cancellationToken);
        Task<ContainerHistoryEntity> Delete(DeleteContainerHistoryModel model, CancellationToken cancellationToken);
        Task<Option<ContainerHistoryEntity>> GetById(Guid id, CancellationToken cancellationToken);
    }
}