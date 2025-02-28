using System.Linq.Expressions;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using AutoMapper;
using DataAccessLayer.Data;
using DataAccessLayer.Entities.ContainerHistories;
using DataAccessLayer.Extensions;
using Domain.ContainersHistory;
using Domain.ContainersHistory.Models;
using Microsoft.EntityFrameworkCore;
using Optional;

namespace DataAccessLayer.Repositories;

public class ContainerHistoryRepository(ApplicationDbContext context, IMapper mapper)
    : IContainerHistoryRepository, IContainerHistoryQueries
{
    public async Task<ContainerHistory> Create(CreateContainerHistoryModel model, CancellationToken cancellationToken)
    {
        var historyEntity = mapper.Map<ContainerHistoryEntity>(model);

        await context.ContainerHistories.AddAuditableAsync(historyEntity, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return mapper.Map<ContainerHistory>(historyEntity);
    }

    public async Task<ContainerHistory> Update(Guid contentId, CancellationToken cancellationToken)
    {
        var containerEntity =
            await context.Containers.FirstOrDefaultAsync(x => x.Id == contentId, cancellationToken);

        var productId = containerEntity!.ProductId;

        var historyEntity =
            context.ContainerHistories.FirstOrDefault(x => x.ProductId == productId && x.EndDate == null);

        historyEntity!.EndDate = DateTime.UtcNow;
        
        context.ContainerHistories.UpdateAuditable(historyEntity);
        await context.SaveChangesAsync(cancellationToken);

        return mapper.Map<ContainerHistory>(historyEntity);
    }

    public async Task<ContainerHistory> Delete(DeleteContainerHistoryModel model, CancellationToken cancellationToken)
    {
        var historyEntity = await GetHistoryAsync(x => x.Id == model.Id, cancellationToken);

        if (historyEntity == null)
        {
            throw new InvalidOperationException("Container history not found.");
        }

        context.ContainerHistories.Remove(historyEntity);
        await context.SaveChangesAsync(cancellationToken);

        return mapper.Map<ContainerHistory>(historyEntity);
    }

    public async Task<IReadOnlyList<ContainerHistory>> GetAll(CancellationToken cancellationToken)
    {
        var historyEntities = await context.ContainerHistories
            .AsNoTracking()
            .Include(h => h.Container)
            .Include(h => h.Product)
            .ToListAsync(cancellationToken);

        return mapper.Map<IReadOnlyList<ContainerHistory>>(historyEntities);
    }

    public async Task<Option<ContainerHistory>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var entity = await GetHistoryAsync(x => x.Id == id, cancellationToken);
        var history = mapper.Map<ContainerHistory>(entity);

        return history == null ? Option.None<ContainerHistory>() : Option.Some(history);
    }

    public async Task<IReadOnlyList<ContainerHistory>> GetByQuery(Guid? containerId, Guid? productId, DateTime? starDate, DateTime? endDate,
        CancellationToken cancellationToken)
    {
        var containersHistory = await context.ContainerHistories.AsNoTracking().ToListAsync(cancellationToken);

        if (containerId.HasValue)
        {
            containersHistory = containersHistory.Where(ch => ch.ContainerId == containerId.Value).ToList();
        }

        if (productId.HasValue)
        {
            containersHistory = containersHistory.Where(ch => ch.ProductId == productId.Value).ToList();
        }

        if (starDate.HasValue)
        {
            containersHistory = containersHistory.Where(ch => ch.StartDate >= starDate).ToList();
        }

        if (endDate.HasValue)
        {
            containersHistory = containersHistory.Where(ch => ch.EndDate <= endDate).ToList();
        }
        
        return mapper.Map<IReadOnlyList<ContainerHistory>>(containersHistory);
    }
    private async Task<ContainerHistoryEntity?> GetHistoryAsync(
        Expression<Func<ContainerHistoryEntity, bool>> predicate,
        CancellationToken cancellationToken,
        bool asNoTracking = false)
    {
        var query = context.ContainerHistories
            .Include(h => h.Container)
            .Include(h => h.Product);

        return asNoTracking
            ? await query.AsNoTracking().FirstOrDefaultAsync(predicate, cancellationToken)
            : await query.FirstOrDefaultAsync(predicate, cancellationToken);
    }
}