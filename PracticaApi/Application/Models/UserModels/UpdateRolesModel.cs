using Domain.Authentications.Users;

namespace Application.Models.UserModels;

public class UpdateRolesModel
{
    public UserId UserId { get; set; }
    public List<string> RoleIds { get; set; } = new();
}