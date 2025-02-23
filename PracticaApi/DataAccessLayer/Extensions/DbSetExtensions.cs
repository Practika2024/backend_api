using DataAccessLayer.Entities.Users;
using Domain.Common.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Extensions;

public static class DbSetExtensions
{
    public static async Task AddAuditableAsync<TEntity>(
        this DbSet<TEntity> dbSet,
        TEntity entity,
        CancellationToken cancellationToken = default)
        where TEntity : AuditableEntity<UserEntity>
    {
        entity.CreatedAt = DateTime.UtcNow;
        entity.ModifiedBy = entity.CreatedBy;
        entity.ModifiedAt = DateTime.UtcNow;

        await dbSet.AddAsync(entity, cancellationToken);
    }
    
    public static async Task UpdateAuditableAsync<TEntity>(
        this DbSet<TEntity> dbSet,
        TEntity entity,
        CancellationToken cancellationToken = default)
        where TEntity : AuditableEntity<UserEntity>
    {
        entity.ModifiedBy = entity.ModifiedBy;
        entity.ModifiedAt = DateTime.UtcNow;

        dbSet.Update(entity);
    }
}