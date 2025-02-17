using Application.Models;
using Application.Models.ContainerModels;
using Domain.Containers;
using Optional;

namespace Application.Common.Interfaces.Repositories
{
    public interface IContainerRepository
    {
        Task<ContainerEntity> Create(CreateContainerModel model, CancellationToken cancellationToken);
        Task<ContainerEntity> Update(UpdateContainerModel model, CancellationToken cancellationToken);
        Task<ContainerEntity> Delete(DeleteContainerModel model, CancellationToken cancellationToken);
        Task<Option<ContainerEntity>> GetById(Guid id, CancellationToken cancellationToken);
        Task<Option<ContainerEntity>> SearchByUniqueCode(string uniqueCode, CancellationToken cancellationToken);
        Task<Option<ContainerEntity>> SearchByName(string name, CancellationToken cancellationToken);
        Task<ContainerEntity> SetContainerContent(SetContainerContentModel model, CancellationToken cancellationToken);
        Task<ContainerEntity> ClearContainerContent(ClearContainerContentModel model, CancellationToken cancellationToken);
    }
}