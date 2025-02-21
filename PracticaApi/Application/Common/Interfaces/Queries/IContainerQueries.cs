using Domain.ContainerModels;
using Optional;

namespace Application.Common.Interfaces.Queries
{
    public interface IContainerQueries
    {
        Task<IReadOnlyList<Container>> GetAll(CancellationToken cancellationToken);
        Task<Option<Container>> GetById(Guid id, CancellationToken cancellationToken);
    }
}