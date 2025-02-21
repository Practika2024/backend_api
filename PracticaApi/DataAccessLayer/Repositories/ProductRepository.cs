using System.Linq.Expressions;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using AutoMapper;
using DataAccessLayer.Data;
using DataAccessLayer.Entities.Products;
using Domain.ProductModels;
using Microsoft.EntityFrameworkCore;
using Optional;

namespace DataAccessLayer.Repositories;

public class ProductRepository(ApplicationDbContext context, IMapper mapper)
    : IProductRepository, IProductQueries
{
    public async Task<Product> Create(CreateProductModel model, CancellationToken cancellationToken)
    {
        var productEntity = mapper.Map<ProductEntity>(model);

        await context.Products.AddAsync(productEntity, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return mapper.Map<Product>(productEntity);
    }

    public async Task<Product> Update(UpdateProductModel model, CancellationToken cancellationToken)
    {
        var productEntity = await GetProductAsync(x => x.Id == model.Id, cancellationToken);

        if (productEntity == null)
        {
            throw new InvalidOperationException("Product not found.");
        }

        productEntity = mapper.Map(model, productEntity);

        await context.SaveChangesAsync(cancellationToken);

        return mapper.Map<Product>(productEntity);
    }

    public async Task<Product> Delete(DeleteProductModel model, CancellationToken cancellationToken)
    {
        var productEntity = await GetProductAsync(x => x.Id == model.Id, cancellationToken);

        if (productEntity == null)
        {
            throw new InvalidOperationException("Product not found.");
        }

        context.Products.Remove(productEntity);
        await context.SaveChangesAsync(cancellationToken);

        return mapper.Map<Product>(productEntity);
    }

    public async Task<IReadOnlyList<Product>> GetAll(CancellationToken cancellationToken)
    {
        var productEntities = await context.Products
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return mapper.Map<IReadOnlyList<Product>>(productEntities);
    }

    public async Task<Option<Product>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var entity = await GetProductAsync(x => x.Id == id, cancellationToken);
        var product = mapper.Map<Product>(entity);
        return product == null ? Option.None<Product>() : Option.Some(product);
    }

    public async Task<Option<Product>> SearchByName(string name, CancellationToken cancellationToken)
    {
        var entity = await GetProductAsync(x => x.Name == name, cancellationToken);
        var product = mapper.Map<Product>(entity);
        return product == null ? Option.None<Product>() : Option.Some(product);
    }

    private async Task<ProductEntity?> GetProductAsync(Expression<Func<ProductEntity, bool>> predicate,
        CancellationToken cancellationToken,
        bool asNoTracking = false)
    {
        if (asNoTracking)
        {
            return await context.Products
                .AsNoTracking()
                .FirstOrDefaultAsync(predicate, cancellationToken);
        }

        return await context.Products
            .FirstOrDefaultAsync(predicate, cancellationToken);
    }
}
