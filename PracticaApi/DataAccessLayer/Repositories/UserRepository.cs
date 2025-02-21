using System.Linq.Expressions;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using AutoMapper;
using DataAccessLayer.Data;
using DataAccessLayer.Entities.Users;
using Domain.UserModels;
using Microsoft.EntityFrameworkCore;
using Optional;

namespace DataAccessLayer.Repositories;

public class UserRepository(ApplicationDbContext context, IMapper mapper) : IUserRepository, IUserQueries
{
    public async Task<User> Create(CreateUserModel model, CancellationToken cancellationToken)
    {
        var userEntity = mapper.Map<UserEntity>(model);

        await context.Users.AddAsync(userEntity, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return mapper.Map<User>(userEntity);
    }

    public async Task<User> Update(UpdateUserModel model, CancellationToken cancellationToken)
    {
        var userEntity = await GetUserAsync(x => x.Id == model.Id, cancellationToken);

        if (userEntity == null)
        {
            throw new InvalidOperationException("User not found.");
        }

        mapper.Map(model, userEntity);
        await context.SaveChangesAsync(cancellationToken);

        return mapper.Map<User>(userEntity);
    }

    public async Task<User> Delete(DeleteUserModel model, CancellationToken cancellationToken)
    {
        var userEntity = await GetUserAsync(x => x.Id == model.Id, cancellationToken);

        if (userEntity == null)
        {
            throw new InvalidOperationException("User not found.");
        }

        context.Users.Remove(userEntity);
        await context.SaveChangesAsync(cancellationToken);

        return mapper.Map<User>(userEntity);
    }

    public async Task<IReadOnlyList<User>> GetAll(CancellationToken cancellationToken)
    {
        var userEntities = await context.Users
            .AsNoTracking()
            .AsSplitQuery()
            .ToListAsync(cancellationToken);

        return mapper.Map<IReadOnlyList<User>>(userEntities);
    }

    public async Task<Option<User>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var entity = await GetUserAsync(x => x.Id == id, cancellationToken);

        var user = mapper.Map<User>(entity);

        return user == null ? Option.None<User>() : Option.Some(user);
    }

    public async Task<Option<User>> SearchByEmail(string email, CancellationToken cancellationToken)
    {
        var entity = await GetUserAsync(x => x.Email == email, cancellationToken);

        var user = mapper.Map<User>(entity);

        return user == null ? Option.None<User>() : Option.Some(user);
    }

    public async Task<Option<User>> SearchByEmailForUpdate(Guid userId, string email, CancellationToken cancellationToken)
    {
        var entity = await GetUserAsync(x => x.Email == email && x.Id != userId, cancellationToken);

        var user = mapper.Map<User>(entity);

        return user == null ? Option.None<User>() : Option.Some(user);
    }

    // public async Task<User> UpdateRoles(UpdateRolesModel model, CancellationToken cancellationToken)
    // {
    //     var userEntity = await GetUserAsync(x => x.Id == model.UserId, cancellationToken);
    //
    //     if (userEntity == null)
    //     {
    //         throw new InvalidOperationException("User not found.");
    //     }
    //
    //     var roleEntity = await context.Roles.FirstOrDefaultAsync(r => r.Id == roleId, cancellationToken);
    //
    //     userEntity.Roles = roleEntities;
    //     await context.SaveChangesAsync(cancellationToken);
    //
    //     return mapper.Map<User>(userEntity);
    // }

    private async Task<UserEntity?> GetUserAsync(Expression<Func<UserEntity, bool>> predicate,
        CancellationToken cancellationToken,
        bool asNoTracking = false)
    {
        if (asNoTracking)
        {
            return await context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(predicate, cancellationToken);
        }

        return await context.Users
            .FirstOrDefaultAsync(predicate, cancellationToken);
    }
}