using Domain.ContainerHistoryModels;
using Optional;

namespace Application.Common.Interfaces.Repositories
{
    public interface IContainerHistoryRepository
    {
        // Task<ContainerHistory> Create(CreateContainerHistoryModel model, CancellationToken cancellationToken);
        // Task<ContainerHistory> Update(UpdateContainerHistoryModel model, CancellationToken cancellationToken);
        // Task<ContainerHistory> Delete(DeleteContainerHistoryModel model, CancellationToken cancellationToken);
        // Task<Option<ContainerHistory>> GetById(Guid id, CancellationToken cancellationToken);
        Task AddProductToContainer(Guid containerId, Guid productId, Guid userId, CancellationToken cancellationToken);
        Task ClearContainerContent(Guid containerId, Guid userId, CancellationToken cancellationToken);
    }
}