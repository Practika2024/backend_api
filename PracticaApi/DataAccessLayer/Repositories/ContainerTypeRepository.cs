using System.Linq.Expressions;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using AutoMapper;
using DataAccessLayer.Data;
using DataAccessLayer.Entities.Containers;
using DataAccessLayer.Entities.Users;
using DataAccessLayer.Extensions;
using Domain.ContainerTypeModels;
using Microsoft.EntityFrameworkCore;
using Optional;

namespace DataAccessLayer.Repositories;

public class ContainerTypeRepository(ApplicationDbContext context, IMapper mapper)
    : IContainerTypeRepository, IContainerTypeQueries
{
    public async Task<ContainerType> Create(CreateContainerTypeModel model, CancellationToken cancellationToken)
    {
        var containerTypeEntity = mapper.Map<ContainerTypeEntity>(model);

        await context.ContainerTypes.AddAuditableAsync(containerTypeEntity, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return mapper.Map<ContainerType>(containerTypeEntity);
    }
    public async Task<ContainerType> Update(UpdateContainerTypeModel model, CancellationToken cancellationToken)
    {
        var containerTypeEntity = await GetContainerTypeAsync(x => x.Id == model.Id, cancellationToken);

        if (containerTypeEntity == null)
        {
            throw new InvalidOperationException("Container type not found.");
        }

        containerTypeEntity = mapper.Map(model, containerTypeEntity);
        
        context.ContainerTypes.UpdateAuditableAsync(containerTypeEntity);

        await context.SaveChangesAsync(cancellationToken);

        return mapper.Map<ContainerType>(containerTypeEntity);
    }

    public async Task<ContainerType> Delete(Guid id, CancellationToken cancellationToken)
    {
        var containerTypeEntity = await GetContainerTypeAsync(x => x.Id == id, cancellationToken);

        if (containerTypeEntity == null)
        {
            throw new InvalidOperationException("Container type not found.");
        }

        context.ContainerTypes.Remove(containerTypeEntity);
        await context.SaveChangesAsync(cancellationToken);

        return mapper.Map<ContainerType>(containerTypeEntity);
    }

    public async Task<IReadOnlyList<ContainerType>> GetAll(CancellationToken cancellationToken)
    {
        var containerTypesEntity = await context.ContainerTypes
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return mapper.Map<IReadOnlyList<ContainerType>>(containerTypesEntity);
    }

    public async Task<Option<ContainerType>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var entity = await GetContainerTypeAsync(x => x.Id == id, cancellationToken);
        var containerType = mapper.Map<ContainerType>(entity);
        return containerType == null ? Option.None<ContainerType>() : Option.Some(containerType);
    }

    public async Task<Option<ContainerType>> SearchByName(string name, CancellationToken cancellationToken)
    {
        var entity = await GetContainerTypeAsync(x => x.Name == name, cancellationToken);
        var containerType = mapper.Map<ContainerType>(entity);
        return containerType == null ? Option.None<ContainerType>() : Option.Some(containerType);
    }

    private async Task<ContainerTypeEntity?> GetContainerTypeAsync(Expression<Func<ContainerTypeEntity, bool>> predicate, CancellationToken cancellationToken,
        bool asNoTracking = false)
    {
        if (asNoTracking)
        {
            return await context.ContainerTypes
                .AsNoTracking()
                .FirstOrDefaultAsync(predicate, cancellationToken);
        }

        return await context.ContainerTypes
            .FirstOrDefaultAsync(predicate, cancellationToken);
    }
}
