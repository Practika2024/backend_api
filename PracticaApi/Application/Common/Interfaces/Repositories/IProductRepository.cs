using Application.Models.ProductModels;
using Domain.Products;
using Optional;

namespace Application.Common.Interfaces.Repositories
{
    public interface IProductRepository
    {
        Task<ProductEntity> Create(CreateProductModel model, CancellationToken cancellationToken);
        Task<ProductEntity> Update(UpdateProductModel model, CancellationToken cancellationToken);
        Task<ProductEntity> Delete(DeleteProductModel model, CancellationToken cancellationToken);
        Task<Option<ProductEntity>> GetById(Guid id, CancellationToken cancellationToken);
    }
}