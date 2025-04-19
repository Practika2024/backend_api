using DataAccessLayer.Entities.RefreshTokens;
using DataAccessLayer.Entities.Roles;
using Domain.Common.Abstractions;

namespace DataAccessLayer.Entities.Users;

public class UserEntity : AuditableEntity
{
    public Guid Id { get; set; }
    public string Email { get; set; }
    public string? Name { get; set; }
    public string? Surname { get; set; }
    public string? Patronymic { get; set; }
    public string PasswordHash { get; set; }
    public string RoleId { get; set; }
    public RoleEntity? Role { get; set; }
    public List<RefreshTokenEntity> RefreshTokens { get; set; } = new();
    public string? ExternalProvider { get; set; }
    public string? ExternalProviderKey { get; set; }
    public bool IsApprovedByAdmin { get; set; }
}