using Domain.Products;
using Optional;

namespace Application.Common.Interfaces.Repositories;
public interface IProductRepository
{
    Task<Product> Create(Product product, CancellationToken cancellationToken);
    Task<Product> Update(Product product, CancellationToken cancellationToken);
    Task<Product> Delete(Product product, CancellationToken cancellationToken);
    Task<Option<Product>> GetById(ProductId id, CancellationToken cancellationToken);
}