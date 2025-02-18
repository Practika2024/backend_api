using Domain.Products;

namespace Application.Exceptions;
public abstract class ProductException(Guid id, string message, Exception innerException = null) : Exception(message, innerException)
{
    public Guid ProductId { get; } = id;
}

public class ProductNotFoundException(Guid id) : ProductException(id, $"Product not found! ID: {id}");