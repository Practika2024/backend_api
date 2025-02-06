using Domain.ContainerHistories;
using Optional;

namespace Application.Common.Interfaces.Repositories;
public interface IContainerHistoryRepository
{
    Task<ContainerHistory> Create(ContainerHistory history, CancellationToken cancellationToken);
    Task<ContainerHistory> Update(ContainerHistory history, CancellationToken cancellationToken);
    Task<ContainerHistory> Delete(ContainerHistory history, CancellationToken cancellationToken);
    Task<Option<ContainerHistory>> GetById(ContainerHistoryId id, CancellationToken cancellationToken);
}