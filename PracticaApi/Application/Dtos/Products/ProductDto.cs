using Domain.Products;

namespace Application.Dtos.Products;

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
            Id = productEntity.Id.Value,
            Name = productEntity.Name,
            Description = productEntity.Description,
            ManufactureDate = productEntity.ManufactureDate
        };
    }
}