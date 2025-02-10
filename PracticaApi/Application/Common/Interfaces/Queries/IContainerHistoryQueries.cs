using Domain.ContainerHistories;
using Domain.Containers;
using Optional;

namespace Application.Common.Interfaces.Queries;
public interface IContainerHistoryQueries
{
    Task<IReadOnlyList<ContainerHistoryEntity>> GetAll(CancellationToken cancellationToken);
    Task<Option<ContainerHistoryEntity>> GetById(ContainerHistoryId id, CancellationToken cancellationToken);
    Task<IReadOnlyList<ContainerHistoryEntity>> GetByContainerId(ContainerId containerId, CancellationToken cancellationToken);
}