using System.Collections.ObjectModel;
using Domain.Authentications.Users;
using Optional;

namespace Application.Common.Interfaces.Repositories;

public interface IUserRepository
{
    Task<UserEntity> Create(UserEntity userEntity, CancellationToken cancellationToken);
    Task<UserEntity> Update(UserEntity userEntity, CancellationToken cancellationToken);
    Task<UserEntity> Delete(UserEntity userEntity, CancellationToken cancellationToken);
    Task<UserEntity> AddRole(UserId userId, string idRole, CancellationToken cancellationToken);
    Task<Option<UserEntity>> GetById(UserId id, CancellationToken cancellationToken);
    Task<Option<UserEntity>> SearchByEmail(string email, CancellationToken cancellationToken);
    Task<Option<UserEntity>> SearchByEmailForUpdate(UserId userId, string email, CancellationToken cancellationToken);
}