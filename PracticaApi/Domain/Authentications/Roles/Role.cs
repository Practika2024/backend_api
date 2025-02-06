using Domain.Authentications.Users;

namespace Domain.Authentications.Roles;

public class Role
{
    public string Id { get; set; }
    public string Name { get; set; }
    public List<User> Users { get; set; } = new();
    
    private Role(string name)
    {
        Id = name;
        Name = name;
    }
    public static Role New(string name)
        => new(name);
}