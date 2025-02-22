using Application.Commands.Containers.Exceptions;

namespace Application.Commands.ContainersType.Exceptions;

public abstract class ContainerTypeException(Guid id, string message, Exception innerException = null) : Exception(message, innerException)
{
    public Guid ContainerId { get; } = id;
}

public class ContainerUnknownException(Guid id, ContainerException innerException)
    : ContainerTypeException(id, $"Unknown exception for the Container under id: {id}", innerException);