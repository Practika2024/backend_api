using System.Linq.Expressions;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Domain.Containers;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Optional;

namespace Infrastructure.Persistence.Repositories;
public class ContainerRepository(ApplicationDbContext _context) : IContainerRepository, IContainerQueries
{
    public async Task<ContainerEntity> Create(ContainerEntity containerEntity, CancellationToken cancellationToken)
    {
        await _context.Containers.AddAsync(containerEntity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return containerEntity;
    }

    public async Task<ContainerEntity> Update(ContainerEntity containerEntity, CancellationToken cancellationToken)
    {
        _context.Containers.Update(containerEntity);
        await _context.SaveChangesAsync(cancellationToken);
        return containerEntity;
    }

    public async Task<ContainerEntity> Delete(ContainerEntity containerEntity, CancellationToken cancellationToken)
    {
        _context.Containers.Remove(containerEntity);
        await _context.SaveChangesAsync(cancellationToken);
        return containerEntity;
    }

    public async Task<IReadOnlyList<ContainerEntity>> GetAll(CancellationToken cancellationToken)
    {
        return await _context.Containers
            .AsNoTracking()
            .Include(c => c.CurrentProduct)
            .Include(c => c.CreatedByNavigation)
            .ToListAsync(cancellationToken);
    }

    public async Task<Option<ContainerEntity>> GetById(ContainerId id, CancellationToken cancellationToken)
    {
        var entity = await GetContainerAsync(x => x.Id == id, cancellationToken);
        return entity == null ? Option.None<ContainerEntity>() : Option.Some(entity);
    }

    public async Task<Option<ContainerEntity>> SearchByName(string name, CancellationToken cancellationToken)
    {
        var entity = await GetContainerAsync(x => x.Name == name, cancellationToken);
        return entity == null ? Option.None<ContainerEntity>() : Option.Some(entity);
    }

    private async Task<ContainerEntity?> GetContainerAsync(Expression<Func<ContainerEntity, bool>> predicate, CancellationToken cancellationToken,
        bool asNoTracking = false)
    {
        if (asNoTracking)
        {
            return await _context.Containers
                .AsNoTracking()
                .Include(c => c.CurrentProduct)
                .FirstOrDefaultAsync(predicate, cancellationToken);
        }
        return await _context.Containers
            .Include(c => c.CurrentProduct)
            .FirstOrDefaultAsync(predicate, cancellationToken);
    }
    public async Task<Option<ContainerEntity>> SearchByUniqueCode(string uniqueCode, CancellationToken cancellationToken)
    {
        var container = await _context.Containers
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.UniqueCode == uniqueCode, cancellationToken);

        return container == null ? Option.None<ContainerEntity>() : Option.Some(container);
    }
}