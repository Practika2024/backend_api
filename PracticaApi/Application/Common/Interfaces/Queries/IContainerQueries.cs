using Domain.Containers;
using Optional;

namespace Application.Common.Interfaces.Queries
{
    public interface IContainerQueries
    {
        Task<IReadOnlyList<ContainerEntity>> GetAll(CancellationToken cancellationToken);
        Task<Option<ContainerEntity>> GetById(ContainerId id, CancellationToken cancellationToken);
    }
}