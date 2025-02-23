namespace Application.Commands.ContainersType.Exceptions;

public abstract class ContainerTypeException(Guid id, string message, Exception innerException = null) : Exception(message, innerException)
{
    public Guid ContainerId { get; } = id;
}

public class ContainerTypeNotFoundException(Guid id) : ContainerTypeException(id, $"Container not found! ID: {id}");
public class ContainerUnknownException(Guid id, ContainerTypeException innerException)
    : ContainerTypeException(id, $"Unknown exception for the Container under id: {id}", innerException);