using Domain.Products;
using Domain.Products.Models;
using Optional;

namespace Application.Common.Interfaces.Repositories
{
    public interface IProductRepository
    {
        Task<Product> Create(CreateProductModel model, CancellationToken cancellationToken);
        Task<Product> Update(UpdateProductModel model, CancellationToken cancellationToken);
        Task<Product> Delete(DeleteProductModel model, CancellationToken cancellationToken);
        Task<Option<Product>> GetById(Guid id, CancellationToken cancellationToken);
    }
}