namespace Application.Exceptions;
public abstract class ContainerHistoryException(Guid id, string message, Exception innerException = null) : Exception(message, innerException)
{
    public Guid HistoryId { get; } = id;
}

public class ContainerHistoryNotFoundException(Guid id) : ContainerHistoryException(id, $"Container history not found! ID: {id}");
public class InvalidProductForHistoryException(Guid productId) : ContainerHistoryException(Guid.Empty, $"Invalid product for history! Product ID: {productId}");
public class InvalidContainerNotFoundException(Guid containerId) : ContainerHistoryException(Guid.Empty, $"Invalid container for history! Container ID: {containerId}");
