using System.Linq.Expressions;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Domain.Products;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Optional;

namespace Infrastructure.Persistence.Repositories;
public class ProductRepository(ApplicationDbContext _context) : IProductRepository, IProductQueries
{
    public async Task<ProductEntity> Create(ProductEntity productEntity, CancellationToken cancellationToken)
    {
        await _context.Products.AddAsync(productEntity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return productEntity;
    }

    public async Task<ProductEntity> Update(ProductEntity productEntity, CancellationToken cancellationToken)
    {
        _context.Products.Update(productEntity);
        await _context.SaveChangesAsync(cancellationToken);
        return productEntity;
    }

    public async Task<ProductEntity> Delete(ProductEntity productEntity, CancellationToken cancellationToken)
    {
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

    public async Task<Option<ProductEntity>> GetById(ProductId id, CancellationToken cancellationToken)
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