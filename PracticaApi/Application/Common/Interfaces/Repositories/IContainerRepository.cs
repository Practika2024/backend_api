namespace Application.Common.Interfaces.Repositories;
using Domain.Containers;
using Optional;

public interface IContainerRepository
{
    Task<ContainerEntity> Create(ContainerEntity containerEntity, CancellationToken cancellationToken);
    Task<ContainerEntity> Update(ContainerEntity containerEntity, CancellationToken cancellationToken);
    Task<ContainerEntity> Delete(ContainerEntity containerEntity, CancellationToken cancellationToken);
    Task<Option<ContainerEntity>> GetById(ContainerId id, CancellationToken cancellationToken);
    Task<Option<ContainerEntity>> SearchByUniqueCode(string uniqueCode, CancellationToken cancellationToken);
    Task<Option<ContainerEntity>> SearchByName(string name, CancellationToken cancellationToken);
}