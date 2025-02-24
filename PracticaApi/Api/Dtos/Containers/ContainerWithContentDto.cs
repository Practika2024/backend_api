namespace Api.Dtos.Containers;

public record ContainerWithContentDto
{
    public Guid? Id { get; init; }
    public string Name { get; init; } = null!;
    public decimal Volume { get; init; }
    public string? Notes { get; init; }
    public string UniqueCode { get; init; }
    public Guid? TypeId { get; init; } = null!;
    public ContentDto? Content { get; init; } = null!;
}

public record ContentDto
{
    public Guid? Id { get; init; }
    public Guid? ProductId { get; init; }
}