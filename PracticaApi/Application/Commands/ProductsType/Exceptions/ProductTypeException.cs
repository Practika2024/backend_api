namespace Application.Commands.ProductsType.Exceptions;

public abstract class ProductTypeException(Guid id, string message, Exception innerException = null) : Exception(message, innerException)
{
    public Guid ProductId { get; } = id;
}

public class ProductTypeNotFoundException(Guid id) : ProductTypeException(id, $"Product not found! ID: {id}");
public class ProductUnknownException(Guid id, ProductTypeException innerException)
    : ProductTypeException(id, $"Unknown exception for the Product under id: {id}", innerException);