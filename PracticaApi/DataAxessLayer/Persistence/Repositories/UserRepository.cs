using System.Linq.Expressions;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Models.UserModels;
using Domain.Authentications.Roles;
using Domain.Authentications.Users;
using Microsoft.EntityFrameworkCore;
using Optional;

namespace Infrastructure.Persistence.Repositories
{
    public class UserRepository(ApplicationDbContext _context) : IUserRepository, IUserQueries
    {
        public async Task<UserEntity> Create(CreateUserModel model, CancellationToken cancellationToken)
        {
            var userEntity = UserEntity.New(
                id: model.Id,
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

        public async Task<UserEntity> AddRole(AddRoleToUserModel model, CancellationToken cancellationToken)
        {
            var userEntity = await GetUserAsync(x => x.Id == model.UserId, cancellationToken);

            if (userEntity == null)
            {
                throw new InvalidOperationException("User not found.");
            }

            var role = await _context.Roles.FirstOrDefaultAsync(x => x.Id == model.RoleId, cancellationToken);

            if (role == null)
            {
                throw new InvalidOperationException("Role not found.");
            }

            userEntity.SetRoles(new List<RoleEntity> { role });

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

        public async Task<Option<UserEntity>> SearchByEmailForUpdate(UserId userId, string email, CancellationToken cancellationToken)
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
        public async Task<UserEntity> UpdateRoles(UpdateRolesModel model, CancellationToken cancellationToken)
        {
            var userEntity = await GetUserAsync(x => x.Id == model.UserId, cancellationToken);
            var roleEntities = new List<RoleEntity>();
            foreach (var roleId in model.RoleIds)
            {
                var role = await _context.Roles.FirstOrDefaultAsync(r => r.Id == roleId, cancellationToken);
                roleEntities.Add(role);
            }
            userEntity.SetRoles(roleEntities);
            await _context.SaveChangesAsync(cancellationToken);
            return userEntity;
        }
        public async Task<UserEntity> UpdateUserImage(UpdateUserImageModel model, CancellationToken cancellationToken)
        {
            var userEntity = await GetUserAsync(x => x.Id == model.UserId, cancellationToken);

            if (userEntity == null)
            {
                throw new InvalidOperationException("User not found.");
            }

            if (!string.IsNullOrEmpty(model.FilePath))
            {
                var userImageEntity = UserImageEntity.New(
                    id: UserImageId.New(),
                    userId: model.UserId,
                    filePath: model.FilePath
                );

                userEntity.UpdateUserImage(userImageEntity);
            }
            else
            {
                userEntity.UpdateUserImage(null);
            }
            await _context.SaveChangesAsync(cancellationToken);
            return userEntity;
        }
    }
}