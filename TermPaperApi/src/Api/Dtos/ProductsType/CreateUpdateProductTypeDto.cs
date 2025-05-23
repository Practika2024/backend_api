namespace Api.Dtos.ProductsType;

public record CreateUpdateProductTypeDto
{
    public string Name { get; init; } = null!;
}