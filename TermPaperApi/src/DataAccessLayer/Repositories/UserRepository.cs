using System.Linq.Expressions;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using AutoMapper;
using DataAccessLayer.Data;
using DataAccessLayer.Entities.Users;
using DataAccessLayer.Extensions;
using Domain.Users;
using Domain.Users.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Optional;

namespace DataAccessLayer.Repositories;

public class UserRepository(ApplicationDbContext context, IMapper mapper) : IUserRepository, IUserQueries
{
    public async Task<User> Create(CreateUserModel model, CancellationToken cancellationToken)
    {
        var userEntity = mapper.Map<UserEntity>(model);

        await context.Users.AddAuditableAsync(userEntity, cancellationToken);
        
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
        context.Users.UpdateAuditable(userEntity);
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

    public async Task<IReadOnlyList<User>> GetAllWithoutApproval(CancellationToken cancellationToken)
    {
        var userEntities = await context.Users
            .AsNoTracking()
            .AsSplitQuery()
            .Where(u=> !u.IsApprovedByAdmin)
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

    public async Task<User> ApproveUser(Guid userId, CancellationToken cancellationToken)
    {
        var entity = await GetUserAsync(x => x.Id == userId, cancellationToken);

        entity!.IsApprovedByAdmin = true;

        await context.SaveChangesAsync(cancellationToken);

        return mapper.Map<User>(entity);
    }

    public async Task<User> VerifyEmailUser(Guid userId, CancellationToken cancellationToken)
    {
        var entity = await GetUserAsync(x => x.Id == userId, cancellationToken);

        entity!.EmailConfirmed = true;

        await context.SaveChangesAsync(cancellationToken);

        return mapper.Map<User>(entity);
    }

    public async Task<Option<User>> SearchByEmailForUpdate(Guid userId, string email, CancellationToken cancellationToken)
    {
        var entity = await GetUserAsync(x => x.Email == email && x.Id != userId, cancellationToken);

        var user = mapper.Map<User>(entity);

        return user == null ? Option.None<User>() : Option.Some(user);
    }

    public async Task<User> UpdateRole(UpdateRoleModel model, CancellationToken cancellationToken)
    {
        var userEntity = await context.Users.FirstOrDefaultAsync(u=> u.Id == model.UserId, cancellationToken);
        
        userEntity!.RoleId = model.RoleId;
        
        await context.SaveChangesAsync(cancellationToken);
    
        return mapper.Map<User>(userEntity);
    }

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
    
    
    public async Task<User?> FindByLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken)
    {
        var userEntity = await context.Users
            .FirstOrDefaultAsync(u => u.ExternalProvider == loginProvider && u.ExternalProviderKey == providerKey, cancellationToken);

        return mapper.Map<User>(userEntity);
    }

    public async Task<User?> FindByEmailAsync(string email, CancellationToken cancellationToken)
    {
        var userEntity = await context.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

        return mapper.Map<User>(userEntity);
    }

    public async Task<IdentityResult> AddLoginAsync(User user, UserLoginInfo loginInfo, CancellationToken cancellationToken)
    {
        var userEntity = await GetUserAsync(x => x.Id == user.Id, cancellationToken);

        if (userEntity == null)
        {
            return IdentityResult.Failed(new IdentityError { Code = "NotFound", Description = "User not found." });
        }
        userEntity.ExternalProvider = loginInfo.LoginProvider;
        userEntity.ExternalProviderKey = loginInfo.ProviderKey;

        await context.SaveChangesAsync(cancellationToken);
        return IdentityResult.Success;
    }

    public async Task<User?> FindUserByEmailVerificationToken(Guid tokenId, CancellationToken cancellationToken)
    {
        var emailVerificationToken = await context.EmailVerificationTokens.FirstOrDefaultAsync(e=> e.Id == tokenId, cancellationToken);
        
        var userEntity = await GetUserAsync(x => x.Id == emailVerificationToken!.UserId, cancellationToken);

        return mapper.Map<User>(userEntity);
    }
}