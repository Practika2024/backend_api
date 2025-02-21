using Domain.ContainerHistories;
using Domain.Containers;
using Domain.Products;
using Domain.RefreshTokens;
using Domain.Reminders;
using Domain.Roles;

namespace Domain.Users;

internal class UserEntity
{
    public Guid Id { get; }
    public string Email { get; set; }
    public string? Name { get; set; }
    public string? Surname { get; set; }
    public string? Patronymic { get; set; }
    public string PasswordHash { get; }
    public string RoleId { get; set; }
    public RoleEntity? Role { get; set; }
    public List<RefreshTokenEntity> RefreshTokens { get; set; } = new();
}