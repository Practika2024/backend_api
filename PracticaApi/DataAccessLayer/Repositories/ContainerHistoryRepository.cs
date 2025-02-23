﻿using System.Linq.Expressions;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using AutoMapper;
using DataAccessLayer.Data;
using DataAccessLayer.Entities.ContainerHistories;
using DataAccessLayer.Extensions;
using Domain.ContainerHistoryModels;
using Microsoft.EntityFrameworkCore;
using Optional;

namespace DataAccessLayer.Repositories;

public class ContainerHistoryRepository(ApplicationDbContext context, IMapper mapper)
    : IContainerHistoryRepository, IContainerHistoryQueries
{
    // public async Task<ContainerHistory> Create(CreateContainerHistoryModel model, CancellationToken cancellationToken)
    // {
    //     var historyEntity = mapper.Map<ContainerHistoryEntity>(model);
    //
    //     await context.ContainerHistories.AddAuditableAsync(historyEntity, cancellationToken);
    //     await context.SaveChangesAsync(cancellationToken);
    //
    //     return mapper.Map<ContainerHistory>(historyEntity);
    // }
    //
    // public async Task<ContainerHistory> Update(UpdateContainerHistoryModel model, CancellationToken cancellationToken)
    // {
    //     var historyEntity = await GetHistoryAsync(x => x.Id == model.Id, cancellationToken);
    //
    //     if (historyEntity == null)
    //     {
    //         throw new InvalidOperationException("Container history not found.");
    //     }
    //
    //     mapper.Map(model, historyEntity);
    //
    //     await context.SaveChangesAsync(cancellationToken);
    //
    //     return mapper.Map<ContainerHistory>(historyEntity);
    // }
    //
    // public async Task<ContainerHistory> Delete(DeleteContainerHistoryModel model, CancellationToken cancellationToken)
    // {
    //     var historyEntity = await GetHistoryAsync(x => x.Id == model.Id, cancellationToken);
    //
    //     if (historyEntity == null)
    //     {
    //         throw new InvalidOperationException("Container history not found.");
    //     }
    //
    //     context.ContainerHistories.Remove(historyEntity);
    //     await context.SaveChangesAsync(cancellationToken);
    //
    //     return mapper.Map<ContainerHistory>(historyEntity);
    // }
    //
    // public async Task<IReadOnlyList<ContainerHistory>> GetAll(CancellationToken cancellationToken)
    // {
    //     var historyEntities = await context.ContainerHistories
    //         .AsNoTracking()
    //         .Include(h => h.Container)
    //         .Include(h => h.Product)
    //         .ToListAsync(cancellationToken);
    //
    //     return mapper.Map<IReadOnlyList<ContainerHistory>>(historyEntities);
    // }
    //
    // public async Task<Option<ContainerHistory>> GetById(Guid id, CancellationToken cancellationToken)
    // {
    //     var entity = await GetHistoryAsync(x => x.Id == id, cancellationToken);
    //     var history = mapper.Map<ContainerHistory>(entity);
    //
    //     return history == null ? Option.None<ContainerHistory>() : Option.Some(history);
    // }
    //
    // public async Task<IReadOnlyList<ContainerHistory>> GetByContainerId(Guid containerId,
    //     CancellationToken cancellationToken)
    // {
    //     var historyEntities = await context.ContainerHistories
    //         .Where(h => h.ContainerId == containerId)
    //         .AsNoTracking()
    //         .Include(h => h.Product)
    //         .ToListAsync(cancellationToken);
    //
    //     return mapper.Map<IReadOnlyList<ContainerHistory>>(historyEntities);
    // }
    //
    // private async Task<ContainerHistoryEntity?> GetHistoryAsync(
    //     Expression<Func<ContainerHistoryEntity, bool>> predicate,
    //     CancellationToken cancellationToken,
    //     bool asNoTracking = false)
    // {
    //     var query = context.ContainerHistories
    //         .Include(h => h.Container)
    //         .Include(h => h.Product);
    //
    //     return asNoTracking
    //         ? await query.AsNoTracking().FirstOrDefaultAsync(predicate, cancellationToken)
    //         : await query.FirstOrDefaultAsync(predicate, cancellationToken);
    // }
    
    public async Task AddProductToContainer(Guid containerId, Guid productId, Guid userId, CancellationToken cancellationToken)
    {
        // Закрити попередній запис історії (якщо існує)
        var activeHistory = await context.ContainerHistories
            .FirstOrDefaultAsync(h => h.ContainerId == containerId && h.EndDate == null, cancellationToken);

        if (activeHistory != null)
        {
            activeHistory.EndDate = DateTime.UtcNow;
            context.Update(activeHistory);
        }

        var newHistory = new ContainerHistoryEntity
        {
            Id = Guid.NewGuid(),
            ContainerId = containerId,
            ProductId = productId,
            StartDate = DateTime.UtcNow,
            CreatedBy = userId,
        };

        await context.ContainerHistories.AddAuditableAsync(newHistory, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }
    public async Task ClearContainerContent(Guid containerId, Guid userId, CancellationToken cancellationToken)
    {
        var activeHistory = await context.ContainerHistories
            .FirstOrDefaultAsync(h => h.ContainerId == containerId && h.EndDate == null, cancellationToken);

        if (activeHistory != null)
        {
            activeHistory.EndDate = DateTime.UtcNow;
            context.Update(activeHistory);
        }

        var newHistory = new ContainerHistoryEntity
        {
            Id = Guid.NewGuid(),
            ContainerId = containerId,
            ProductId = null,
            StartDate = DateTime.UtcNow,
            CreatedBy = userId,
        };

        await context.ContainerHistories.AddAuditableAsync(newHistory, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }
    
    public async Task<List<ContainerHistory>> GetContainerHistory(Guid containerId, CancellationToken cancellationToken)
    {
        var history = await context.ContainerHistories
            .Where(h => h.ContainerId == containerId)
            .Include(h => h.Product)
            .OrderBy(h => h.StartDate)
            .ToListAsync(cancellationToken);

        return mapper.Map<List<ContainerHistory>>(history);
    }
}