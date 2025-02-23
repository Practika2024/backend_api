using Domain.ContainerHistoryModels;
using Optional;

namespace Application.Common.Interfaces.Queries
{
    public interface IContainerHistoryQueries
    {
        Task<IReadOnlyList<ContainerHistory>> GetAll(CancellationToken cancellationToken);
        Task<Option<ContainerHistory>> GetById(Guid id, CancellationToken cancellationToken);
        Task<IReadOnlyList<ContainerHistory>> GetByContainerId(Guid containerId, CancellationToken cancellationToken);
    }
}