using Domain.Users;
using Optional;

namespace Application.Common.Interfaces.Queries
{
    public interface IUserQueries
    {
        Task<IReadOnlyList<UserEntity>> GetAll(CancellationToken cancellationToken);
        Task<Option<UserEntity>> GetById(Guid id, CancellationToken cancellationToken);
        Task<Option<UserEntity>> SearchByEmail(string email, CancellationToken cancellationToken);
    }
}