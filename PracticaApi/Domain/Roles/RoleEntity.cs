namespace Domain.Roles;

public class RoleEntity
{
    public string Id { get; set; }
    public string Name { get; set; }
    
    private RoleEntity(string name)
    {
        Id = name;
        Name = name;
    }
    public static RoleEntity New(string name)
        => new(name);
}