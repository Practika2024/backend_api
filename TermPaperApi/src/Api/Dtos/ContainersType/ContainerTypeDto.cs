namespace Api.Dtos.ContainersType;

public record ContainerTypeDto
{
    public Guid? Id { get; init; }
    public string Name { get; init; } = null!;
}