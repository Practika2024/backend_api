namespace Api.Dtos.Products;

public record ProductImageDto
{
    public Guid Id { get; init; }
    public string FilePath { get; init; } = null!;
    public string FileName { get; init; } = null!;
}