using Domain.ProductTypes;
using Domain.ProductTypes.Models;
using Optional;

namespace Application.Common.Interfaces.Repositories
{
    public interface IProductTypeRepository
    {
        Task<ProductType> Create(CreateProductTypeModel model, CancellationToken cancellationToken);
        Task<ProductType> Update(UpdateProductTypeModel model, CancellationToken cancellationToken);
        Task<ProductType> Delete(Guid id, CancellationToken cancellationToken);
        Task<Option<ProductType>> GetById(Guid id, CancellationToken cancellationToken);
        Task<Option<ProductType>> SearchByName(string name, CancellationToken cancellationToken);
    }
}