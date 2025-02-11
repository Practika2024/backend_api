using Application.Models.UserModels;
using Domain.Authentications.Users;
using Optional;

namespace Application.Common.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task<UserEntity> Create(CreateUserModel model, CancellationToken cancellationToken);
        Task<UserEntity> Update(UpdateUserModel model, CancellationToken cancellationToken);
        Task<UserEntity> Delete(DeleteUserModel model, CancellationToken cancellationToken);
        Task<UserEntity> AddRole(AddRoleToUserModel model, CancellationToken cancellationToken);
        Task<Option<UserEntity>> GetById(UserId id, CancellationToken cancellationToken);
        Task<Option<UserEntity>> SearchByEmail(string email, CancellationToken cancellationToken);
        Task<Option<UserEntity>> SearchByEmailForUpdate(UserId userId, string email, CancellationToken cancellationToken);
        Task<UserEntity> UpdateRoles(UpdateRolesModel model, CancellationToken cancellationToken);
        Task<UserEntity> UpdateUserImage(UpdateUserImageModel model, CancellationToken cancellationToken);
    }
}