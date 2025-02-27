namespace Domain.Users.Models;

public class UpdateRolesModel
{
    public Guid UserId { get; set; }
    public List<string> RoleIds { get; set; } = new();
}