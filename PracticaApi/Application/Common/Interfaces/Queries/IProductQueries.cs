using Domain.Products;
using Optional;

namespace Application.Common.Interfaces.Queries
{
    public interface IProductQueries
    {
        Task<IReadOnlyList<Product>> GetAll(CancellationToken cancellationToken);
        Task<Option<Product>> GetById(Guid id, CancellationToken cancellationToken);
        Task<Option<Product>> SearchByName(string name, CancellationToken cancellationToken);
    }
}