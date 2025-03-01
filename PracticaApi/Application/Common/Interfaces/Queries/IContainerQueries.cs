using Domain.Containers;
using Optional;

namespace Application.Common.Interfaces.Queries;

public interface IContainerQueries
{
    Task<IReadOnlyList<Container>> GetAll(CancellationToken cancellationToken);
    Task<Option<Container>> GetById(Guid id, CancellationToken cancellationToken);
    Task<Option<Container>> GetByUniqueCode(string uniqueCode, CancellationToken cancellationToken);
    Task<bool> IsProductInContainer(Guid productId, CancellationToken cancellationToken);
    Task<bool> IsContainerIsEmpty(Guid containerId, CancellationToken cancellationToken);
    
    
    Task<IReadOnlyList<Container>> GetContainersByFillStatus(bool isEmpty, CancellationToken cancellationToken);
    Task<IReadOnlyList<Container>> GetContainersByProductType(Guid productTypeId, CancellationToken cancellationToken);
    Task<IReadOnlyList<Container>> GetContainersByProduct(Guid productId, CancellationToken cancellationToken);
    Task<IReadOnlyList<Container>> GetEmptyContainersByLastProduct(Guid lastProductId, CancellationToken cancellationToken);
}