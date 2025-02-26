using Domain.ContainersHistory;
using Optional;

namespace Application.Common.Interfaces.Queries
{
    public interface IContainerHistoryQueries
    {
        Task<IReadOnlyList<ContainerHistory>> GetAll(CancellationToken cancellationToken);
        Task<Option<ContainerHistory>> GetById(Guid id, CancellationToken cancellationToken);
        // Task<IReadOnlyList<ContainerHistory>> GetByContainerId(Guid containerId, CancellationToken cancellationToken);
        //
        // Task<IReadOnlyList<ContainerHistory>> GetByContainerIdFromTo(Guid containerId, DateTime startDate,
        //     DateTime endDate, CancellationToken cancellationToken);
        //
        // Task<IReadOnlyList<ContainerHistory>> GetByProductId(Guid productId, CancellationToken cancellationToken);
        //
        // Task<IReadOnlyList<ContainerHistory>> GetByProductIdFromTo(Guid productId, DateTime startDate,
        //     DateTime endDate, CancellationToken cancellationToken);
        //
        // Task<IReadOnlyList<ContainerHistory>> GetByContainerIdAndProductId(Guid containerId, Guid productId,
        //     CancellationToken cancellationToken);
        //
        // Task<IReadOnlyList<ContainerHistory>> GetByContainerIdAndProductIdFromTo(Guid containerId, Guid productId, 
        //     DateTime startDate, DateTime endDate, CancellationToken cancellationToken);
        
        Task<IReadOnlyList<ContainerHistory>> GetByQuery(Guid? containerId, Guid? productId, DateTime? starDate, DateTime? endDate, CancellationToken cancellationToken);
    }
}