using System.Linq.Expressions;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using AutoMapper;
using DataAccessLayer.Data;
using DataAccessLayer.Entities.Products;
using DataAccessLayer.Extensions;
using Domain.ProductTypes.Models;
using Microsoft.EntityFrameworkCore;
using Optional;
using ProductType = Domain.ProductTypes.ProductType;

namespace DataAccessLayer.Repositories;

public class ProductTypeRepository(ApplicationDbContext context, IMapper mapper)
    : IProductTypeRepository, IProductTypeQueries
{
    public async Task<ProductType> Create(CreateProductTypeModel model, CancellationToken cancellationToken)
    {
        var productTypeEntity = mapper.Map<ProductTypeEntity>(model);

        await context.ProductTypes.AddAuditableAsync(productTypeEntity, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return mapper.Map<ProductType>(productTypeEntity);
    }
    public async Task<ProductType> Update(UpdateProductTypeModel model, CancellationToken cancellationToken)
    {
        var productTypeEntity = await GetProductTypeAsync(x => x.Id == model.Id, cancellationToken);

        if (productTypeEntity == null)
        {
            throw new InvalidOperationException("Product type not found.");
        }

        productTypeEntity = mapper.Map(model, productTypeEntity);
        
        context.ProductTypes.UpdateAuditable(productTypeEntity);

        await context.SaveChangesAsync(cancellationToken);

        return mapper.Map<ProductType>(productTypeEntity);
    }

    public async Task<ProductType> Delete(Guid id, CancellationToken cancellationToken)
    {
        var productTypeEntity = await GetProductTypeAsync(x => x.Id == id, cancellationToken);

        if (productTypeEntity == null)
        {
            throw new InvalidOperationException("Product type not found.");
        }

        context.ProductTypes.Remove(productTypeEntity);
        await context.SaveChangesAsync(cancellationToken);

        return mapper.Map<ProductType>(productTypeEntity);
    }

    public async Task<IReadOnlyList<ProductType>> GetAll(CancellationToken cancellationToken)
    {
        var productTypesEntity = await context.ProductTypes
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return mapper.Map<IReadOnlyList<ProductType>>(productTypesEntity);
    }

    public async Task<Option<ProductType>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var entity = await GetProductTypeAsync(x => x.Id == id, cancellationToken);
        var productType = mapper.Map<ProductType>(entity);
        return productType == null ? Option.None<ProductType>() : Option.Some(productType);
    }

    public async Task<Option<ProductType>> SearchByName(string name, CancellationToken cancellationToken)
    {
        var entity = await GetProductTypeAsync(x => x.Name == name, cancellationToken);
        var productType = mapper.Map<ProductType>(entity);
        return productType == null ? Option.None<ProductType>() : Option.Some(productType);
    }

    private async Task<ProductTypeEntity?> GetProductTypeAsync(Expression<Func<ProductTypeEntity, bool>> predicate, CancellationToken cancellationToken,
        bool asNoTracking = false)
    {
        if (asNoTracking)
        {
            return await context.ProductTypes
                .AsNoTracking()
                .FirstOrDefaultAsync(predicate, cancellationToken);
        }

        return await context.ProductTypes
            .FirstOrDefaultAsync(predicate, cancellationToken);
    }
}
