using Domain.Products;
using Optional;

namespace Application.Common.Interfaces.Repositories;
public interface IProductRepository
{
    Task<ProductEntity> Create(ProductEntity productEntity, CancellationToken cancellationToken);
    Task<ProductEntity> Update(ProductEntity productEntity, CancellationToken cancellationToken);
    Task<ProductEntity> Delete(ProductEntity productEntity, CancellationToken cancellationToken);
    Task<Option<ProductEntity>> GetById(ProductId id, CancellationToken cancellationToken);
}