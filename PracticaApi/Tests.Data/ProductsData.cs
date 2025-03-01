using Domain.Products;

namespace Tests.Data;

public static class ProductsData
{
    public static Product MainProduct => new()
    {
        Id = Guid.NewGuid(),
        Name = "Main Test Product",
        Description = "Description for main test product",
        ManufactureDate = DateTime.UtcNow,
        TypeId = Guid.NewGuid()
    };

    public static Product NewProduct(string name, string? description, DateTime manufactureDate, Guid typeId) => new()
    {
        Id = Guid.NewGuid(),
        Name = name,
        Description = description,
        ManufactureDate = manufactureDate,
        TypeId = typeId
    };
}