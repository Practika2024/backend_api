using Domain.ContainerTypes;
using Optional;

namespace Application.Common.Interfaces.Queries
{
    public interface IContainerTypeQueries
    {
        Task<IReadOnlyList<ContainerType>> GetAll(CancellationToken cancellationToken);
        Task<Option<ContainerType>> GetById(Guid id, CancellationToken cancellationToken);
    }
}