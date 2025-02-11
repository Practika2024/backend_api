using Domain.Authentications.Users;

namespace Domain.Containers;

public class ContainerTypeEntity
{
    public ContainerTypeId Id { get; private set; }
    public string Name { get; private set; }

    private ContainerTypeEntity(ContainerTypeId id, string name)
    {
        Id = id;
        Name = name;
    }

    public static ContainerTypeEntity New(string name)
    {
        return new ContainerTypeEntity(ContainerTypeId.New(), name);
    }

    public void UpdateName(string newName)
    {
        Name = newName;
    }
}

public record ContainerTypeId(Guid Value)
{
    public static ContainerTypeId New() => new(Guid.NewGuid());
    public static ContainerTypeId Empty => new(Guid.Empty);
    public override string ToString() => Value.ToString();
}