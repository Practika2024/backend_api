using Domain.Products;
using Optional;

namespace Application.Common.Interfaces.Queries;
public interface IProductQueries
{
    Task<IReadOnlyList<ProductEntity>> GetAll(CancellationToken cancellationToken);
    Task<Option<ProductEntity>> GetById(ProductId id, CancellationToken cancellationToken);
    Task<Option<ProductEntity>> SearchByName(string name, CancellationToken cancellationToken);
}