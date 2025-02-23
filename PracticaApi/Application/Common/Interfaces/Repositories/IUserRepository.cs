using Domain.UserModels;
using Optional;

namespace Application.Common.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task<User> Create(CreateUserModel model, CancellationToken cancellationToken);
        Task<User> Update(UpdateUserModel model, CancellationToken cancellationToken);
        Task<User> Delete(DeleteUserModel model, CancellationToken cancellationToken);
        // Task<User> AddRole(AddRoleToUserModel model, CancellationToken cancellationToken);
        Task<Option<User>> GetById(Guid id, CancellationToken cancellationToken);
        Task<Option<User>> SearchByEmail(string email, CancellationToken cancellationToken);
        Task<Option<User>> SearchByEmailForUpdate(Guid userId, string email, CancellationToken cancellationToken);
        //Task<User> UpdateRoles(UpdateRolesModel model, CancellationToken cancellationToken);
    }
}