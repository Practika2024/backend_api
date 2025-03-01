using Domain.Containers;

namespace Tests.Data;

public class ContainersData
{
    public static Container? MainContainer(Guid id) => new()
    {
        Id = Guid.NewGuid(),
        Name = "Test Container",
        Notes = "Test Notes",
        Volume = 10, 
        TypeId = id,
    };
}