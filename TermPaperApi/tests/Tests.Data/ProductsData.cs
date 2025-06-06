﻿using Domain.Products;

namespace Tests.Data;

public static class ProductsData
{
    public static Product MainProduct(Guid productTypeId) => new()
    {
        Id = Guid.NewGuid(),
        Name = "Main Test Product",
        Description = "Description for main test product",
        ManufactureDate = DateTime.UtcNow,
        TypeId = productTypeId
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