using Domain.Authentications.Roles;
using Optional;

namespace Application.Common.Interfaces.Queries;

public interface IRoleQueries
{
    Task<IReadOnlyList<RoleEntity>> GetAll(CancellationToken cancellationToken);
    Task<Option<RoleEntity>> GetByName(string name, CancellationToken cancellationToken);
}