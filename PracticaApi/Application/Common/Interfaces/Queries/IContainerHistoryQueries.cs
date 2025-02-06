using Domain.ContainerHistories;
using Domain.Containers;
using Optional;

namespace Application.Common.Interfaces.Queries;
public interface IContainerHistoryQueries
{
    Task<IReadOnlyList<ContainerHistory>> GetAll(CancellationToken cancellationToken);
    Task<Option<ContainerHistory>> GetById(ContainerHistoryId id, CancellationToken cancellationToken);
    Task<IReadOnlyList<ContainerHistory>> GetByContainerId(ContainerId containerId, CancellationToken cancellationToken);
}