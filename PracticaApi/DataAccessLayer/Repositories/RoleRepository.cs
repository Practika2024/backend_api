using System.Linq.Expressions;
using Application.Common.Interfaces.Queries;
using AutoMapper;
using DataAccessLayer.Data;
using DataAccessLayer.Entities.Roles;
using Domain.RoleModels;
using Microsoft.EntityFrameworkCore;
using Optional;

namespace DataAccessLayer.Repositories;

public class RoleRepository(ApplicationDbContext context, IMapper mapper) : IRoleQueries
{
    public async Task<IReadOnlyList<Role>> GetAll(CancellationToken cancellationToken)
    {
        var roleEntities = await context.Roles
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return mapper.Map<IReadOnlyList<Role>>(roleEntities);
    }

    public async Task<Option<Role>> GetByName(string name, CancellationToken cancellationToken)
    {
        var entity = await GetRoleAsync(r => r.Name == name, cancellationToken, true);

        var role = mapper.Map<Role>(entity);

        return role == null ? Option.None<Role>() : Option.Some(role);
    }

    private async Task<RoleEntity?> GetRoleAsync(Expression<Func<RoleEntity, bool>> predicate,
        CancellationToken cancellationToken, bool asNoTracking = true)
    {
        if (asNoTracking)
        {
            return await context.Roles
                .AsNoTracking()
                .FirstOrDefaultAsync(predicate, cancellationToken);
        }

        return await context.Roles
            .FirstOrDefaultAsync(predicate, cancellationToken);
    }
}