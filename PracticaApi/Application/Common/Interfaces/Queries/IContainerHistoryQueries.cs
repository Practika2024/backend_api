using Domain.ContainerHistories;
using Domain.Containers;
using Optional;

namespace Application.Common.Interfaces.Queries
{
    public interface IContainerHistoryQueries
    {
        Task<IReadOnlyList<ContainerHistoryEntity>> GetAll(CancellationToken cancellationToken);
        Task<Option<ContainerHistoryEntity>> GetById(Guid id, CancellationToken cancellationToken);
        Task<IReadOnlyList<ContainerHistoryEntity>> GetByContainerId(Guid containerId, CancellationToken cancellationToken);
    }
}