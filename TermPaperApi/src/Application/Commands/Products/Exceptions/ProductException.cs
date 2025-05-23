namespace Application.Commands.Products.Exceptions;

public abstract class ProductException(Guid id, string message, Exception innerException = null) : Exception(message, innerException)
{
    public Guid ProductId { get; } = id;
}

public class ProductNotFoundException(Guid id) : ProductException(id, $"Product not found! ID: {id}");
public class ProductAlreadyExistsException(Guid id) : ProductException(id, $"Product already exists: {id}");
public class ProductTypeNotFoundException(Guid id) : ProductException(id, $"Product type not found! ID: {id}");
public class UserNotFoundException(Guid id) : ProductException(id, $"User not found! ID: {id}");

public class ImageSaveException(Guid id)
    : ProductException(id, $"Product under id: {id} have problems with images save!");

public class ProductUnknownException(Guid id, ProductException innerException)
    : ProductException(id, $"Unknown exception for the Product under id: {id}", innerException);