using Application.Models;
using Application.Models.ContainerModels;
using Application.Models.ContainerTypeModels;
using Domain.Containers;
using Optional;

namespace Application.Common.Interfaces.Repositories
{
    public interface IContainerTypeRepository
    {
        Task<ContainerTypeEntity> Create(CreateContainerTypeModel model, CancellationToken cancellationToken);
        Task<ContainerTypeEntity> Update(UpdateContainerTypeModel model, CancellationToken cancellationToken);
        Task<ContainerTypeEntity> Delete(Guid id, CancellationToken cancellationToken);
        Task<Option<ContainerTypeEntity>> GetById(Guid id, CancellationToken cancellationToken);
        Task<Option<ContainerTypeEntity>> SearchByName(string name, CancellationToken cancellationToken);
    }
}