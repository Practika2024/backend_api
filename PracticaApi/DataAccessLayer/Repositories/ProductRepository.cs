using System.Linq.Expressions;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Models.ProductModels;
using DataAccessLayer.Data;
using Domain.Products;
using Microsoft.EntityFrameworkCore;
using Optional;

namespace DataAccessLayer.Repositories
{
    public class ProductRepository(ApplicationDbContext _context) : IProductRepository, IProductQueries
    {
        public async Task<ProductEntity> Create(CreateProductModel model, CancellationToken cancellationToken)
        {
            var productEntity = ProductEntity.New(
                id: model.Id,
                name: model.Name,
                description: model.Description,
                manufactureDate: model.ManufactureDate,
                createdBy: model.CreatedBy,
                typeId: model.TypeId
            );

            await _context.Products.AddAsync(productEntity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return productEntity;
        }

        public async Task<ProductEntity> Update(UpdateProductModel model, CancellationToken cancellationToken)
        {
            var productEntity = await GetProductAsync(x => x.Id == model.Id, cancellationToken);

            if (productEntity == null)
            {
                throw new InvalidOperationException("Product not found.");
            }

            productEntity.Update(
                name: model.Name,
                description: model.Description,
                manufactureDate: model.ManufactureDate,
                modifiedBy: model.ModifiedBy,
                typeId: model.TypeId
            );

            await _context.SaveChangesAsync(cancellationToken);

            return productEntity;
        }

        public async Task<ProductEntity> Delete(DeleteProductModel model, CancellationToken cancellationToken)
        {
            var productEntity = await GetProductAsync(x => x.Id == model.Id, cancellationToken);

            if (productEntity == null)
            {
                throw new InvalidOperationException("Product not found.");
            }

            _context.Products.Remove(productEntity);
            await _context.SaveChangesAsync(cancellationToken);

            return productEntity;
        }

        public async Task<IReadOnlyList<ProductEntity>> GetAll(CancellationToken cancellationToken)
        {
            return await _context.Products
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<Option<ProductEntity>> GetById(Guid id, CancellationToken cancellationToken)
        {
            var entity = await GetProductAsync(x => x.Id == id, cancellationToken);
            return entity == null ? Option.None<ProductEntity>() : Option.Some(entity);
        }

        public async Task<Option<ProductEntity>> SearchByName(string name, CancellationToken cancellationToken)
        {
            var entity = await GetProductAsync(x => x.Name == name, cancellationToken);
            return entity == null ? Option.None<ProductEntity>() : Option.Some(entity);
        }

        private async Task<ProductEntity?> GetProductAsync(Expression<Func<ProductEntity, bool>> predicate, CancellationToken cancellationToken,
            bool asNoTracking = false)
        {
            if (asNoTracking)
            {
                return await _context.Products
                    .AsNoTracking()
                    .FirstOrDefaultAsync(predicate, cancellationToken);
            }

            return await _context.Products
                .FirstOrDefaultAsync(predicate, cancellationToken);
        }
    }
}