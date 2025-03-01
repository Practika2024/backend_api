using Domain.Common.Abstractions;
using Domain.RefreshTokens;
using Domain.Roles;

namespace Domain.Users;

public class User : AuditableEntity<User>
{
    public Guid Id { get; set; }
    public string Email { get; set; }
    public string? Name { get; set; }
    public string? Surname { get; set; }
    public string? Patronymic { get; set; }
    public string PasswordHash { get; set; }
    public string RoleId { get; set; }
    public Role? Role { get; set; }
    public List<RefreshToken> RefreshTokens { get; set; } = new();
    public string? ExternalProvider { get; set; }
    public string? ExternalProviderKey { get; set; }
}