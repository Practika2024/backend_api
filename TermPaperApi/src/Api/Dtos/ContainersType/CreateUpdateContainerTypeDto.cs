namespace Api.Dtos.ContainersType;

public record CreateUpdateContainerTypeDto
{
    public string Name { get; init; } = null!;
}