﻿using Domain.ContainersHistory;
using Optional;

namespace Application.Common.Interfaces.Queries
{
    public interface IContainerHistoryQueries
    {
        Task<IReadOnlyList<ContainerHistory>> GetAll(CancellationToken cancellationToken);
        Task<Option<ContainerHistory>> GetById(Guid id, CancellationToken cancellationToken);
        
        Task<IReadOnlyList<ContainerHistory>> GetByQuery(Guid? containerId, Guid? productId, DateTime? starDate, DateTime? endDate, CancellationToken cancellationToken);
    }
}