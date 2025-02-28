using Domain.Containers;
using Optional;

namespace Application.Common.Interfaces.Queries
{
    public interface IContainerQueries
    {
        Task<IReadOnlyList<Container>> GetAll(CancellationToken cancellationToken);
        Task<Option<Container>> GetById(Guid id, CancellationToken cancellationToken);
        Task<Option<Container>> GetByUniqueCode(string uniqueCode, CancellationToken cancellationToken);
        Task<bool> IsProductInContainer(Guid productId, CancellationToken cancellationToken);
        Task<bool> IsContainerIsEmpty(Guid containerId, CancellationToken cancellationToken);
    }
}