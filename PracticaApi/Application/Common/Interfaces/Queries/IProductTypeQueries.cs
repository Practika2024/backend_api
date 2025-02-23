using Domain.ProductModels;
using Domain.ProductTypeModels;
using Optional;

namespace Application.Common.Interfaces.Queries
{
    public interface IProductTypeQueries
    {
        Task<IReadOnlyList<ProductType>> GetAll(CancellationToken cancellationToken);
        Task<Option<ProductType>> GetById(Guid id, CancellationToken cancellationToken);
    }
}