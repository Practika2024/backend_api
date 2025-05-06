using Domain.Users;
using Domain.Users.Models;
using Microsoft.AspNetCore.Identity;
using Optional;

namespace Application.Common.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task<User> Create(CreateUserModel model, CancellationToken cancellationToken);
        Task<User> Update(UpdateUserModel model, CancellationToken cancellationToken);
        Task<User> Delete(DeleteUserModel model, CancellationToken cancellationToken);
        Task<Option<User>> GetById(Guid id, CancellationToken cancellationToken);
        Task<Option<User>> SearchByEmail(string email, CancellationToken cancellationToken);
        Task<Option<User>> SearchByEmailForUpdate(Guid userId, string email, CancellationToken cancellationToken);
        Task<User> UpdateRole(UpdateRoleModel model, CancellationToken cancellationToken);
        Task<User> ApproveUser(Guid userId, bool isUserApproved, CancellationToken cancellationToken);
        Task<User> VerifyEmailUser(Guid userId, CancellationToken cancellationToken);

        
        Task<User?> FindByLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken);
        Task<IdentityResult> AddLoginAsync(User user, UserLoginInfo loginInfo, CancellationToken cancellationToken);
        Task<User?> FindUserByEmailVerificationToken(Guid tokenId, CancellationToken cancellationToken);
    }
}