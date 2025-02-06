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
    public async Task<Container> Create(Container container, CancellationToken cancellationToken)
    {
        await _context.Containers.AddAsync(container, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return container;
    }

    public async Task<Container> Update(Container container, CancellationToken cancellationToken)
    {
        _context.Containers.Update(container);
        await _context.SaveChangesAsync(cancellationToken);
        return container;
    }

    public async Task<Container> Delete(Container container, CancellationToken cancellationToken)
    {
        _context.Containers.Remove(container);
        await _context.SaveChangesAsync(cancellationToken);
        return container;
    }

    public async Task<IReadOnlyList<Container>> GetAll(CancellationToken cancellationToken)
    {
        return await _context.Containers
            .AsNoTracking()
            .Include(c => c.CurrentProduct)
            .Include(c => c.CreatedByNavigation)
            .ToListAsync(cancellationToken);
    }

    public async Task<Option<Container>> GetById(ContainerId id, CancellationToken cancellationToken)
    {
        var entity = await GetContainerAsync(x => x.Id == id, cancellationToken);
        return entity == null ? Option.None<Container>() : Option.Some(entity);
    }

    public async Task<Option<Container>> SearchByName(string name, CancellationToken cancellationToken)
    {
        var entity = await GetContainerAsync(x => x.Name == name, cancellationToken);
        return entity == null ? Option.None<Container>() : Option.Some(entity);
    }

    private async Task<Container?> GetContainerAsync(Expression<Func<Container, bool>> predicate, CancellationToken cancellationToken,
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
    public async Task<Option<Container>> SearchByUniqueCode(string uniqueCode, CancellationToken cancellationToken)
    {
        var container = await _context.Containers
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.UniqueCode == uniqueCode, cancellationToken);

        return container == null ? Option.None<Container>() : Option.Some(container);
    }
}