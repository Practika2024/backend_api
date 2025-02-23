namespace Application.Commands.Containers.Exceptions;

public abstract class ContainerException(Guid id, string message, Exception innerException = null) : Exception(message, innerException)
{
    public Guid ContainerId { get; } = id;
}

public class ContainerNotFoundException(Guid id) : ContainerException(id, $"Container not found! ID: {id}");
public class ContainerByThisUniqueCodeAlreadyExistsException(string uniqueCode) : ContainerException(Guid.Empty, $"Container by this unique code already exists! Code: {uniqueCode}");
public class ContainerCreationException() : ContainerException(Guid.Empty, $"Container by this unique code already exists! Code: ");
public class ContainerAlreadyExistsException(Guid id) : ContainerException(id, $"Container already exists: {id}");
public class ContainerTypeNotFoundException(Guid id) : ContainerException(id, $"Container type not found! ID: {id}");
public class UserNotFoundException(Guid id) : ContainerException(id, $"User not found! ID: {id}");

public class ProductForContainerNotFoundException(Guid id) : ContainerException(id, $"Not found product for container under id: {id}");
public class ContainerUnknownException(Guid id, ContainerException innerException)
    : ContainerException(id, $"Unknown exception for the Container under id: {id}", innerException);