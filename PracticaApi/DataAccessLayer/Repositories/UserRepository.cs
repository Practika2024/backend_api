using System.Linq.Expressions;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Models.UserModels;
using DataAccessLayer.Data;
using Domain.Roles;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using Optional;

namespace DataAccessLayer.Repositories
{
    public class UserRepository(ApplicationDbContext _context) : IUserRepository, IUserQueries
    {
        public async Task<UserEntity> Create(CreateUserModel model, CancellationToken cancellationToken)
        {
            var userEntity = UserEntity.New(
                id: model.Id,
                roleId: model.RoleId,
                email: model.Email,
                name: model.Name,
                surname: model.Surname,
                patronymic: model.Patronymic,
                passwordHash: model.PasswordHash
            );

            await _context.Users.AddAsync(userEntity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return userEntity;
        }

        public async Task<UserEntity> Update(UpdateUserModel model, CancellationToken cancellationToken)
        {
            var userEntity = await GetUserAsync(x => x.Id == model.Id, cancellationToken);

            if (userEntity == null)
            {
                throw new InvalidOperationException("User not found.");
            }
            userEntity.UpdateUser(email: model.Email, name: model.Name, surname: model.Surname, patronymic: model.Patronymic);
            await _context.SaveChangesAsync(cancellationToken);

            return userEntity;
        }

        public async Task<UserEntity> Delete(DeleteUserModel model, CancellationToken cancellationToken)
        {
            var userEntity = await GetUserAsync(x => x.Id == model.Id, cancellationToken);

            if (userEntity == null)
            {
                throw new InvalidOperationException("User not found.");
            }

            _context.Users.Remove(userEntity);
            await _context.SaveChangesAsync(cancellationToken);

            return userEntity;
        }

        // public async Task<UserEntity> AddRole(AddRoleToUserModel model, CancellationToken cancellationToken)
        // {
        //     var userEntity = await GetUserAsync(x => x.Id == model.UserId, cancellationToken);
        //
        //     if (userEntity == null)
        //     {
        //         throw new InvalidOperationException("User not found.");
        //     }
        //
        //     var role = await _context.Roles.FirstOrDefaultAsync(x => x.Id == model.RoleId, cancellationToken);
        //
        //     if (role == null)
        //     {
        //         throw new InvalidOperationException("Role not found.");
        //     }
        //
        //     userEntity.SetRoles(new List<RoleEntity> { role });
        //
        //     await _context.SaveChangesAsync(cancellationToken);
        //
        //     return userEntity;
        // }

        public async Task<IReadOnlyList<UserEntity>> GetAll(CancellationToken cancellationToken)
        {
            return await _context.Users
                .AsNoTracking()
                .AsSplitQuery()
                .ToListAsync(cancellationToken);
        }

        public async Task<Option<UserEntity>> GetById(Guid id, CancellationToken cancellationToken)
        {
            var entity = await GetUserAsync(x => x.Id == id, cancellationToken);
            return entity == null ? Option.None<UserEntity>() : Option.Some(entity);
        }

        public async Task<Option<UserEntity>> SearchByEmail(string email, CancellationToken cancellationToken)
        {
            var entity = await GetUserAsync(x => x.Email == email, cancellationToken);
            return entity == null ? Option.None<UserEntity>() : Option.Some(entity);
        }

        public async Task<Option<UserEntity>> SearchByEmailForUpdate(Guid userId, string email, CancellationToken cancellationToken)
        {
            var entity = await GetUserAsync(x => x.Email == email && x.Id != userId, cancellationToken);
            return entity == null ? Option.None<UserEntity>() : Option.Some(entity);
        }

        private async Task<UserEntity?> GetUserAsync(Expression<Func<UserEntity, bool>> predicate, CancellationToken cancellationToken,
            bool asNoTracking = false)
        {
            if (asNoTracking)
            {
                return await _context.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(predicate, cancellationToken);
            }

            return await _context.Users
                .FirstOrDefaultAsync(predicate, cancellationToken);
        }
        public async Task<UserEntity> UpdateRoles(UpdateRolesModel model, CancellationToken cancellationToken)
        {
            var userEntity = await GetUserAsync(x => x.Id == model.UserId, cancellationToken);
            var roleEntities = new List<RoleEntity>();
            foreach (var roleId in model.RoleIds)
            {
                var role = await _context.Roles.FirstOrDefaultAsync(r => r.Id == roleId, cancellationToken);
                roleEntities.Add(role);
            }
            await _context.SaveChangesAsync(cancellationToken);
            return userEntity;
        }
    }
}