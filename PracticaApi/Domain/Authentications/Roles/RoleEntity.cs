using Domain.Authentications.Users;

namespace Domain.Authentications.Roles;

public class RoleEntity
{
    public string Id { get; set; }
    public string Name { get; set; }
    public List<UserEntity> Users { get; set; } = new();
    
    private RoleEntity(string name)
    {
        Id = name;
        Name = name;
    }
    public static RoleEntity New(string name)
        => new(name);
}