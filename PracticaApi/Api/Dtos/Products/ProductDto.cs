using Domain.Products;

namespace Api.Dtos.Products;

public record ProductDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = null!;
    public string? Description { get; init; }
    public DateTime ManufactureDate { get; init; }

    public static ProductDto FromDomainModel(ProductEntity productEntity)
    {
        return new ProductDto
        {
            Id = productEntity.Id,
            Name = productEntity.Name,
            Description = productEntity.Description,
            ManufactureDate = productEntity.ManufactureDate
        };
    }
}