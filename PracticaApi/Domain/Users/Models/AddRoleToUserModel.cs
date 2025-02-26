namespace Domain.Users.Models;

public class AddRoleToUserModel
{
    public Guid UserId { get; set; }
    public string RoleId { get; set; }
}