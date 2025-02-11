using Domain.Containers;

namespace Application.Commands.Containers.Exceptions;

public abstract class ContainerException(ContainerId id, string message, Exception innerException = null) : Exception(message, innerException)
{
    public ContainerId ContainerId { get; } = id;
}

public class ContainerNotFoundException(ContainerId id) : ContainerException(id, $"Container not found! ID: {id}");
public class ContainerByThisUniqueCodeAlreadyExistsException(string uniqueCode) : ContainerException(ContainerId.Empty, $"Container by this unique code already exists! Code: {uniqueCode}");
public class ContainerCreationException() : ContainerException(ContainerId.Empty, $"Container by this unique code already exists! Code: ");
public class ContainerAlreadyExistsException(ContainerId id) : ContainerException(id, $"Container already exists: {id}");

public class ProductForContainerNotFoundException(ContainerId id) : ContainerException(id, $"Not found product for container under id: {id}");
public class ContainerUnknownException(ContainerId id, ContainerException innerException)
    : ContainerException(id, $"Unknown exception for the Container under id: {id}", innerException);