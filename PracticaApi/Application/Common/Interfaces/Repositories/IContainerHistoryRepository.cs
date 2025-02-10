using Domain.ContainerHistories;
using Optional;

namespace Application.Common.Interfaces.Repositories;
public interface IContainerHistoryRepository
{
    Task<ContainerHistoryEntity> Create(ContainerHistoryEntity historyEntity, CancellationToken cancellationToken);
    Task<ContainerHistoryEntity> Update(ContainerHistoryEntity historyEntity, CancellationToken cancellationToken);
    Task<ContainerHistoryEntity> Delete(ContainerHistoryEntity historyEntity, CancellationToken cancellationToken);
    Task<Option<ContainerHistoryEntity>> GetById(ContainerHistoryId id, CancellationToken cancellationToken);
}