using System.Linq.Expressions;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using AutoMapper;
using DataAccessLayer.Data;
using DataAccessLayer.Entities.Containers;
using Domain.ContainerModels;
using Microsoft.EntityFrameworkCore;
using Optional;

namespace DataAccessLayer.Repositories;

public class ContainerRepository(ApplicationDbContext context, IMapper mapper)
    : IContainerRepository, IContainerQueries
{
    public async Task<Container> Create(CreateContainerModel model, CancellationToken cancellationToken)
    {
        var containerEntity = mapper.Map<ContainerEntity>(model);

        await context.Containers.AddAsync(containerEntity, cancellationToken);
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

        containerEntity = mapper.Map<ContainerEntity>(model);

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
            .Include(c => c.Content)
            .ToListAsync(cancellationToken);

        return mapper.Map<IReadOnlyList<Container>>(containersEntity);
    }

    public async Task<Option<Container>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var entity = await GetContainerAsync(x => x.Id == id, cancellationToken);

        var container = mapper.Map<Container>(entity);

        return container == null ? Option.None<Container>() : Option.Some(container);
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

        var contentEntity = mapper.Map<ContainerContentEntity>(model);

        containerEntity.Content = contentEntity;

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

        containerEntity.Content = null;

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
        bool asNoTracking = false)
    {
        if (asNoTracking)
        {
            return await context.Containers
                .AsNoTracking()
                .Include(c => c.Content)
                .FirstOrDefaultAsync(predicate, cancellationToken);
        }

        return await context.Containers
            .Include(c => c.Content)
            .FirstOrDefaultAsync(predicate, cancellationToken);
    }
}