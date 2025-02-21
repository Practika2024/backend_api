using Domain.Containers;
using Optional;

namespace Application.Common.Interfaces.Queries
{
    public interface IContainerTypeQueries
    {
        Task<IReadOnlyList<ContainerTypeEntity>> GetAll(CancellationToken cancellationToken);
        Task<Option<ContainerTypeEntity>> GetById(Guid id, CancellationToken cancellationToken);
    }
}