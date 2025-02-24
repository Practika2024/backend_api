using Domain.ContainerTypeModels;

namespace Tests.Data;

public static class ContainerTypeData
{
    public static ContainerType MainContainerType => new()
    {
        Id = Guid.NewGuid(),
        Name = "Main Test Container Type"
    };
}