using System.Linq.Expressions;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Domain.Authentications.Users;
using Microsoft.EntityFrameworkCore;
using Optional;

namespace Infrastructure.Persistence.Repositories;

public class UserRepository(ApplicationDbContext _context) : IUserRepository, IUserQueries
{
    public async Task<UserEntity> Create(UserEntity userEntity, CancellationToken cancellationToken)
    {
        await _context.Users.AddAsync(userEntity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return userEntity;
    }

    public async Task<UserEntity> Update(UserEntity userEntity, CancellationToken cancellationToken)
    {
        _context.Users.Update(userEntity);
        await _context.SaveChangesAsync(cancellationToken);
        return userEntity;
    }

    public async Task<UserEntity> Delete(UserEntity userEntity, CancellationToken cancellationToken)
    {
        _context.Users.Remove(userEntity);
        await _context.SaveChangesAsync(cancellationToken);
        return userEntity;
    }

    public async Task<IReadOnlyList<UserEntity>> GetAll(CancellationToken cancellationToken)
    {
        return await _context.Users
            .AsNoTracking()
            .Include(x => x.Roles)
            .Include(u => u.UserImage)
            .AsSplitQuery()
            .ToListAsync(cancellationToken);
    }
    
    public async Task<Option<UserEntity>> GetById(UserId id, CancellationToken cancellationToken)
    {
        var entity = await GetUserAsync(x => x.Id == id, cancellationToken);

        return entity == null ? Option.None<UserEntity>() : Option.Some(entity);
    }
    
    public async Task<Option<UserEntity>> SearchByEmail(string email, CancellationToken cancellationToken)
    {
        var entity = await GetUserAsync(x => x.Email == email, cancellationToken);

        return entity == null ? Option.None<UserEntity>() : Option.Some(entity);
    }

    public async Task<Option<UserEntity>> SearchByEmailForUpdate(UserId userId, string email,
        CancellationToken cancellationToken)
    {
        var entity = await GetUserAsync(x => x.Email == email && x.Id != userId, cancellationToken);

        return entity == null ? Option.None<UserEntity>() : Option.Some(entity);
    }

    public async Task<UserEntity> AddRole(UserId userId, string idRole, CancellationToken cancellationToken)
    {
        var entity = await GetUserAsync(x => x.Id == userId, cancellationToken);

        var role = await _context.Roles.FirstOrDefaultAsync(x => x.Id == idRole, cancellationToken);
        entity.Roles.Add(role);
        await _context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task<UserEntity?> GetUserAsync(Expression<Func<UserEntity, bool>> predicate, CancellationToken cancellationToken,
        bool asNoTracking = false)
    {
        if (asNoTracking)
        {
            return await _context.Users
                .Include(u => u.Roles)
                .Include(u => u.UserImage)
                .AsNoTracking()
                .FirstOrDefaultAsync(predicate, cancellationToken);
        }

        return await _context.Users
            .Include(u => u.Roles)
            .Include(u => u.UserImage)
            .FirstOrDefaultAsync(predicate, cancellationToken);
    }
}