using Domain.Authentications.Users;

namespace Application.Models.UserModels;

public class AddRoleToUserModel
{
    public UserId UserId { get; set; }
    public string RoleId { get; set; }
}