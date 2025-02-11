using Domain.Authentications.Users;

namespace Application.Models.UserModels;

public class UpdateUserModel
{
    public UserId Id { get; set; }
    public string Email { get; set; }
    public string? Name { get; set; }
    public string? Surname { get; set; }
    public string? Patronymic { get; set; }
}