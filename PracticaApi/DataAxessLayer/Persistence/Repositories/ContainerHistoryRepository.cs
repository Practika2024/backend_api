using System.Linq.Expressions;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Domain.ContainerHistories;
using Domain.Containers;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Optional;

namespace Infrastructure.Persistence.Repositories;
public class ContainerHistoryRepository(ApplicationDbContext _context) : IContainerHistoryRepository, IContainerHistoryQueries
{
    public async Task<ContainerHistoryEntity> Create(ContainerHistoryEntity historyEntity, CancellationToken cancellationToken)
    {
        await _context.ContainerHistories.AddAsync(historyEntity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return historyEntity;
    }

    public async Task<ContainerHistoryEntity> Update(ContainerHistoryEntity historyEntity, CancellationToken cancellationToken)
    {
        _context.ContainerHistories.Update(historyEntity);
        await _context.SaveChangesAsync(cancellationToken);
        return historyEntity;
    }

    public async Task<ContainerHistoryEntity> Delete(ContainerHistoryEntity historyEntity, CancellationToken cancellationToken)
    {
        _context.ContainerHistories.Remove(historyEntity);
        await _context.SaveChangesAsync(cancellationToken);
        return historyEntity;
    }

    public async Task<IReadOnlyList<ContainerHistoryEntity>> GetAll(CancellationToken cancellationToken)
    {
        return await _context.ContainerHistories
            .AsNoTracking()
            .Include(h => h.Container)
            .Include(h => h.Product)
            .ToListAsync(cancellationToken);
    }

    public async Task<Option<ContainerHistoryEntity>> GetById(ContainerHistoryId id, CancellationToken cancellationToken)
    {
        var entity = await GetHistoryAsync(x => x.Id == id, cancellationToken);
        return entity == null ? Option.None<ContainerHistoryEntity>() : Option.Some(entity);
    }

    public async Task<IReadOnlyList<ContainerHistoryEntity>> GetByContainerId(ContainerId containerId, CancellationToken cancellationToken)
    {
        return await _context.ContainerHistories
            .Where(h => h.ContainerId == containerId)
            .AsNoTracking()
            .Include(h => h.Product)
            .ToListAsync(cancellationToken);
    }

    private async Task<ContainerHistoryEntity?> GetHistoryAsync(Expression<Func<ContainerHistoryEntity, bool>> predicate, CancellationToken cancellationToken,
        bool asNoTracking = false)
    {
        if (asNoTracking)
        {
            return await _context.ContainerHistories
                .AsNoTracking()
                .Include(h => h.Container)
                .Include(h => h.Product)
                .FirstOrDefaultAsync(predicate, cancellationToken);
        }
        return await _context.ContainerHistories
            .Include(h => h.Container)
            .Include(h => h.Product)
            .FirstOrDefaultAsync(predicate, cancellationToken);
    }
}