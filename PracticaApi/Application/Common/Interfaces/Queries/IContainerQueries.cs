using Domain.Containers;
using Optional;

namespace Application.Common.Interfaces.Queries;

public interface IContainerQueries
{
    Task<IReadOnlyList<Container>> GetAll(CancellationToken cancellationToken);
    Task<Option<Container>> GetById(ContainerId id, CancellationToken cancellationToken);
 
}