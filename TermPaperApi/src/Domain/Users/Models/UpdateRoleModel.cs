namespace Domain.Users.Models;

public class UpdateRoleModel
{
    public Guid UserId { get; set; }
    public string RoleId { get; set; }
}