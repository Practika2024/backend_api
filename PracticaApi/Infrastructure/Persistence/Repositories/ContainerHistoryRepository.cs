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
    public async Task<ContainerHistory> Create(ContainerHistory history, CancellationToken cancellationToken)
    {
        await _context.ContainerHistories.AddAsync(history, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return history;
    }

    public async Task<ContainerHistory> Update(ContainerHistory history, CancellationToken cancellationToken)
    {
        _context.ContainerHistories.Update(history);
        await _context.SaveChangesAsync(cancellationToken);
        return history;
    }

    public async Task<ContainerHistory> Delete(ContainerHistory history, CancellationToken cancellationToken)
    {
        _context.ContainerHistories.Remove(history);
        await _context.SaveChangesAsync(cancellationToken);
        return history;
    }

    public async Task<IReadOnlyList<ContainerHistory>> GetAll(CancellationToken cancellationToken)
    {
        return await _context.ContainerHistories
            .AsNoTracking()
            .Include(h => h.Container)
            .Include(h => h.Product)
            .ToListAsync(cancellationToken);
    }

    public async Task<Option<ContainerHistory>> GetById(ContainerHistoryId id, CancellationToken cancellationToken)
    {
        var entity = await GetHistoryAsync(x => x.Id == id, cancellationToken);
        return entity == null ? Option.None<ContainerHistory>() : Option.Some(entity);
    }

    public async Task<IReadOnlyList<ContainerHistory>> GetByContainerId(ContainerId containerId, CancellationToken cancellationToken)
    {
        return await _context.ContainerHistories
            .Where(h => h.ContainerId == containerId)
            .AsNoTracking()
            .Include(h => h.Product)
            .ToListAsync(cancellationToken);
    }

    private async Task<ContainerHistory?> GetHistoryAsync(Expression<Func<ContainerHistory, bool>> predicate, CancellationToken cancellationToken,
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