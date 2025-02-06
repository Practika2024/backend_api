namespace Application.Common.Interfaces.Repositories;
using Domain.Containers;
using Optional;

public interface IContainerRepository
{
    Task<Container> Create(Container container, CancellationToken cancellationToken);
    Task<Container> Update(Container container, CancellationToken cancellationToken);
    Task<Container> Delete(Container container, CancellationToken cancellationToken);
    Task<Option<Container>> GetById(ContainerId id, CancellationToken cancellationToken);
    Task<Option<Container>> SearchByUniqueCode(string uniqueCode, CancellationToken cancellationToken);
    Task<Option<Container>> SearchByName(string name, CancellationToken cancellationToken);
}