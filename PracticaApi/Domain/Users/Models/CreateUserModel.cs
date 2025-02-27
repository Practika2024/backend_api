namespace Domain.Users.Models;

public class CreateUserModel
{
    public Guid Id { get; set; }
    public string RoleId { get; set; }
    public string Email { get; set; }
    public string? Name { get; set; }
    public string? Surname { get; set; }
    public string? Patronymic { get; set; }
    public string PasswordHash { get; set; }
}