using Domain.Products;

namespace Application.Exceptions;
public abstract class ProductException(ProductId id, string message, Exception innerException = null) : Exception(message, innerException)
{
    public ProductId ProductId { get; } = id;
}

public class ProductNotFoundException(ProductId id) : ProductException(id, $"Product not found! ID: {id}");