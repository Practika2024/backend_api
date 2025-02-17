using System.Linq.Expressions;
using Application.Common.Interfaces.Queries;
using DataAccessLayer.Data;
using Domain.Roles;
using Microsoft.EntityFrameworkCore;
using Optional;

namespace DataAccessLayer.Repositories;

public class RoleRepository(ApplicationDbContext _context) : IRoleQueries
{
    public async Task<IReadOnlyList<RoleEntity>> GetAll(CancellationToken cancellationToken)
    {
        return await _context.Roles
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<Option<RoleEntity>> GetByName(string name, CancellationToken cancellationToken)
    {
        var entity = await GetRoleAsync(r => r.Name == name, cancellationToken, false);
        
        return entity == null ? Option.None<RoleEntity>() : Option.Some(entity);
    }
    
    public async Task<RoleEntity?> GetRoleAsync(Expression<Func<RoleEntity, bool>> predicate, CancellationToken cancellationToken, bool asNoTracking = true)
    {
        if (asNoTracking)
        {
            return await _context.Roles
                .AsNoTracking()
                .FirstOrDefaultAsync(predicate, cancellationToken);
        }

        return await _context.Roles
            .FirstOrDefaultAsync(predicate, cancellationToken);
    }
}