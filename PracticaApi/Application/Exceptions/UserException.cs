namespace Application.Exceptions;

public abstract class UserException(Guid id, string message, Exception innerException = null)
    : Exception(message, innerException)
{
    public Guid UserId { get; } = id;
}

public class UserByThisEmailAlreadyExistsException(Guid id)
    : UserException(id, $"User by this email already exists! User id: {id}");

public class EmailOrPasswordAreIncorrect() : UserException(Guid.Empty, "Email or Password are incorrect!");

public class UserNotFoundException(Guid id) : UserException(id, $"User under id: {id} was not found!");

public class ImageSaveException(Guid id) : UserException(id, $"User under id: {id} have problems with image save!");

public class RoleNotFoundException(string role)
    : UserException(Guid.Empty, $"Roles under name: {role} was not found!");

public class UserUnknownException(Guid id, Exception innerException)
    : UserException(id, $"Unknown exception for the user under id: {id}", innerException);
    