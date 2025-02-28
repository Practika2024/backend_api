using System.Linq.Expressions;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using AutoMapper;
using DataAccessLayer.Data;
using DataAccessLayer.Entities.Containers;
using DataAccessLayer.Extensions;
using Domain.Containers;
using Domain.Containers.Models;
using Microsoft.EntityFrameworkCore;
using Optional;

namespace DataAccessLayer.Repositories;

public class ContainerRepository(ApplicationDbContext context, IMapper mapper)
    : IContainerRepository, IContainerQueries
{
    public async Task<Container> Create(CreateContainerModel model, CancellationToken cancellationToken)
    {
        var containerEntity = mapper.Map<ContainerEntity>(model);
        await context.Containers.AddAuditableAsync(containerEntity, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return mapper.Map<Container>(containerEntity);
    }

    public async Task<Container> Update(UpdateContainerModel model, CancellationToken cancellationToken)
    {
        var containerEntity = await GetContainerAsync(x => x.Id == model.Id, cancellationToken);

        if (containerEntity == null)
        {
            throw new InvalidOperationException("Container not found.");
        }

        context.Entry(containerEntity).CurrentValues.SetValues(model);
    
        context.Containers.UpdateAuditable(containerEntity);
        await context.SaveChangesAsync(cancellationToken);

        return mapper.Map<Container>(containerEntity);
    }

    public async Task<Container> Delete(DeleteContainerModel model, CancellationToken cancellationToken)
    {
        var containerEntity = await GetContainerAsync(x => x.Id == model.Id, cancellationToken);

        if (containerEntity == null)
        {
            throw new InvalidOperationException("Container not found.");
        }

        context.Containers.Remove(containerEntity);
        await context.SaveChangesAsync(cancellationToken);

        return mapper.Map<Container>(containerEntity);
    }

    public async Task<IReadOnlyList<Container>> GetAll(CancellationToken cancellationToken)
    {
        var containersEntity = await context.Containers
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return mapper.Map<IReadOnlyList<Container>>(containersEntity);
    }

    public async Task<Option<Container>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var entity = await GetContainerAsync(x => x.Id == id, cancellationToken, true);

        var container = mapper.Map<Container>(entity);

        return container == null ? Option.None<Container>() : Option.Some(container);
    }

    public async Task<Option<Container>> GetByUniqueCode(string uniqueCode, CancellationToken cancellationToken)
    {
        var entity = await GetContainerAsync(x => x.UniqueCode == uniqueCode, cancellationToken);

        var container = mapper.Map<Container>(entity);

        return container == null ? Option.None<Container>() : Option.Some(container);
    }

    public async Task<bool> IsProductInContainer(Guid productId, CancellationToken cancellationToken)
    {
       return await context.Containers.AnyAsync(x => x.ProductId == productId, cancellationToken);
    }

    public async Task<bool> IsContainerIsEmpty(Guid containerId, CancellationToken cancellationToken)
    {
        var containerContent = await context.Containers.FirstOrDefaultAsync(x => x.Id == containerId, cancellationToken);
        return containerContent?.ProductId == null;
    }

    public async Task<Option<Container>> SearchByName(string name, CancellationToken cancellationToken)
    {
        var entity = await GetContainerAsync(x => x.Name == name, cancellationToken);

        var container = mapper.Map<Container>(entity);

        return container == null ? Option.None<Container>() : Option.Some(container);
    }

    public async Task<Option<Container>> SearchByUniqueCode(string uniqueCode, CancellationToken cancellationToken)
    {
        var entity = await context.Containers
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.UniqueCode == uniqueCode, cancellationToken);

        var container = mapper.Map<Container>(entity);

        return container == null ? Option.None<Container>() : Option.Some(container);
    }

    public async Task<Container> SetContainerContent(SetContainerContentModel model,
        CancellationToken cancellationToken)
    {
        var containerEntity = await GetContainerAsync(x => x.Id == model.ContainerId, cancellationToken);

        if (containerEntity == null)
        {
            throw new InvalidOperationException("Container not found.");
        }

        containerEntity.ModifiedBy = model.ModifiedBy;
        containerEntity.ProductId = model.ProductId;

        context.Containers.UpdateAuditable(containerEntity);

        await context.SaveChangesAsync(cancellationToken);
        return mapper.Map<Container>(containerEntity);
    }

    public async Task<Container> ClearContainerContent(ClearContainerContentModel model,
        CancellationToken cancellationToken)
    {
        var containerEntity = await GetContainerAsync(x => x.Id == model.ContainerId, cancellationToken);

        if (containerEntity == null)
        {
            throw new InvalidOperationException("Container not found.");
        }

        containerEntity.ModifiedBy = model.ModifiedBy;
        containerEntity.ProductId = null;

        context.Containers.UpdateAuditable(containerEntity);
        await context.SaveChangesAsync(cancellationToken);

        return mapper.Map<Container>(containerEntity);
    }

    public async Task<int> GetLastSequenceForPrefixAsync(string codePrefix, CancellationToken cancellationToken)
    {
        var containers = await context.Containers
            .Where(c => c.UniqueCode.StartsWith(codePrefix))
            .ToListAsync(cancellationToken);

        if (!containers.Any())
        {
            return 0;
        }

        var maxSequence = containers
            .Select(c =>
            {
                var numericPart = c.UniqueCode.Substring(codePrefix.Length);
                return int.TryParse(numericPart, out int number) ? number : 0;
            })
            .Max();

        return maxSequence;
    }

    private async Task<ContainerEntity?> GetContainerAsync(Expression<Func<ContainerEntity, bool>> predicate,
        CancellationToken cancellationToken,
        bool asNotTracking = false)
    {
        if (asNotTracking)
        {
            return await context.Containers
                .AsNoTracking()
                .FirstOrDefaultAsync(predicate, cancellationToken);
        }

        return await context.Containers
            .FirstOrDefaultAsync(predicate, cancellationToken);
    }
}