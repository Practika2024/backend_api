﻿using DataAccessLayer.Entities.Users;
using Domain.Common.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Extensions;

public static class DbSetExtensions
{
    public static async Task AddAuditableAsync<TEntity>(
        this DbSet<TEntity> dbSet,
        TEntity entity,
        CancellationToken cancellationToken = default)
        where TEntity : AuditableEntity
    {
        entity.CreatedAt = DateTime.UtcNow;
        entity.ModifiedBy = entity.CreatedBy;
        entity.ModifiedAt = DateTime.UtcNow;

        await dbSet.AddAsync(entity, cancellationToken);
    }
    
    public static void UpdateAuditable<TEntity>(
        this DbSet<TEntity> dbSet,
        TEntity entity)
        where TEntity : AuditableEntity
    {
        entity.ModifiedBy = entity.ModifiedBy;
        entity.ModifiedAt = DateTime.UtcNow;

        dbSet.Update(entity);
    }
}