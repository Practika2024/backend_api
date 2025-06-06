﻿using Domain.Users;
using Optional;

namespace Application.Common.Interfaces.Queries
{
    public interface IUserQueries
    {
        Task<IReadOnlyList<User>> GetAll(CancellationToken cancellationToken);
        Task<IReadOnlyList<User>> GetAllWithoutApproval(CancellationToken cancellationToken);
        Task<Option<User>> GetById(Guid id, CancellationToken cancellationToken);
        Task<Option<User>> SearchByEmail(string email, CancellationToken cancellationToken);
        Task<string> GetEmailByUserId(Guid userId, CancellationToken cancellationToken);
        Task<Option<User>> GetByIdAsQuery(Guid id, CancellationToken cancellationToken);
    }
}