﻿using Domain.Products;

namespace Api.Dtos.Products;

public record ProductDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = null!;
    public string? Description { get; init; }
    public DateTime ManufactureDate { get; init; }
    public Guid TypeId { get; init; }
    public List<ProductImageDto>? Images { get; init; }
}