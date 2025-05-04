namespace Api.Dtos.ProductsType;

public record ProductTypeDto
{
    public Guid? Id { get; init; }
    public string Name { get; init; } = null!;
}