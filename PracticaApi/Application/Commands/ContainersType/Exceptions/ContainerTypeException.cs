namespace Application.Commands.ContainersType.Exceptions;

public abstract class ContainerTypeException(Guid typeId, string message, Exception innerException = null) : Exception(message, innerException)
{
    public Guid ContainerTypeId { get; } = typeId;
}

public class ContainerTypeNotFoundException(Guid typeId) : ContainerTypeException(typeId, $"Container type not found! ID: {typeId}");
public class ContainerTypeAlreadyExists(Guid typeId) : ContainerTypeException(typeId, $"Container type already exists! ID: {typeId}");
public class ContainerUnknownException(Guid typeId, ContainerTypeException innerException)
    : ContainerTypeException(typeId, $"Unknown exception for the Container type under id: {typeId}", innerException);