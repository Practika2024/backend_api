using Domain.ProductTypeModels;

namespace Tests.Data;

public class ProductTypeData
{
    public static ProductType MainProductType => new()
    {
        Id = Guid.NewGuid(),
        Name = "Main Test Product Type"
    };
}