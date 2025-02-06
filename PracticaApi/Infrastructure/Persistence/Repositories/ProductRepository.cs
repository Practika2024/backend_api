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
    public async Task<Product> Create(Product product, CancellationToken cancellationToken)
    {
        await _context.Products.AddAsync(product, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return product;
    }

    public async Task<Product> Update(Product product, CancellationToken cancellationToken)
    {
        _context.Products.Update(product);
        await _context.SaveChangesAsync(cancellationToken);
        return product;
    }

    public async Task<Product> Delete(Product product, CancellationToken cancellationToken)
    {
        _context.Products.Remove(product);
        await _context.SaveChangesAsync(cancellationToken);
        return product;
    }

    public async Task<IReadOnlyList<Product>> GetAll(CancellationToken cancellationToken)
    {
        return await _context.Products
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<Option<Product>> GetById(ProductId id, CancellationToken cancellationToken)
    {
        var entity = await GetProductAsync(x => x.Id == id, cancellationToken);
        return entity == null ? Option.None<Product>() : Option.Some(entity);
    }

    public async Task<Option<Product>> SearchByName(string name, CancellationToken cancellationToken)
    {
        var entity = await GetProductAsync(x => x.Name == name, cancellationToken);
        return entity == null ? Option.None<Product>() : Option.Some(entity);
    }

    private async Task<Product?> GetProductAsync(Expression<Func<Product, bool>> predicate, CancellationToken cancellationToken,
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