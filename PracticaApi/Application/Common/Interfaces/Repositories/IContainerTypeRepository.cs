using Domain.ContainerTypeModels;
using Optional;

namespace Application.Common.Interfaces.Repositories
{
    public interface IContainerTypeRepository
    {
        Task<ContainerType> Create(CreateContainerTypeModel model, CancellationToken cancellationToken);
        Task<ContainerType> Update(UpdateContainerTypeModel model, CancellationToken cancellationToken);
        Task<ContainerType> Delete(Guid id, CancellationToken cancellationToken);
        Task<Option<ContainerType>> GetById(Guid id, CancellationToken cancellationToken);
        Task<Option<ContainerType>> SearchByName(string name, CancellationToken cancellationToken);
    }
}