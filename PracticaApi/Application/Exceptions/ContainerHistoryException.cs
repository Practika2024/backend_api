using Domain.ContainerHistories;
using Domain.Containers;
using Domain.Products;

namespace Application.Exceptions;
public abstract class ContainerHistoryException(ContainerHistoryId id, string message, Exception innerException = null) : Exception(message, innerException)
{
    public ContainerHistoryId HistoryId { get; } = id;
}

public class ContainerHistoryNotFoundException(ContainerHistoryId id) : ContainerHistoryException(id, $"Container history not found! ID: {id}");
public class InvalidProductForHistoryException(ProductId productId) : ContainerHistoryException(ContainerHistoryId.Empty, $"Invalid product for history! Product ID: {productId}");
public class InvalidContainerNotFoundException(ContainerId containerId) : ContainerHistoryException(ContainerHistoryId.Empty, $"Invalid container for history! Container ID: {containerId}");
