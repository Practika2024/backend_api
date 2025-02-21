using Domain.ContainerModels;
using Optional;

namespace Application.Common.Interfaces.Repositories
{
    public interface IContainerRepository
    {
        Task<Container> Create(CreateContainerModel model, CancellationToken cancellationToken);
        Task<Container> Update(UpdateContainerModel model, CancellationToken cancellationToken);
        Task<Container> Delete(DeleteContainerModel model, CancellationToken cancellationToken);
        Task<Option<Container>> GetById(Guid id, CancellationToken cancellationToken);
        Task<Option<Container>> SearchByUniqueCode(string uniqueCode, CancellationToken cancellationToken);
        Task<Option<Container>> SearchByName(string name, CancellationToken cancellationToken);
        Task<Container> SetContainerContent(SetContainerContentModel model, CancellationToken cancellationToken);
        Task<Container> ClearContainerContent(ClearContainerContentModel model, CancellationToken cancellationToken);
        Task<int> GetLastSequenceForPrefixAsync(string codePrefix, CancellationToken cancellationToken);
    }
}