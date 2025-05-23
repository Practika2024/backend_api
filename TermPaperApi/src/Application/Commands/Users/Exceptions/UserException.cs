namespace Application.Commands.Users.Exceptions;

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

public class EmailVerificationException(Guid id) : UserException(id, $"User under id: {id} can't be verified! Token is expired!");
public class EmailVerificationNotFoundException() : UserException(Guid.Empty, $"Email verification token not found!");

public class RoleNotFoundException(string role)
    : UserException(Guid.Empty, $"Roles under name: {role} was not found!");

public class UserUnknownException(Guid id, Exception innerException)
    : UserException(id, $"Unknown exception for the user under id: {id}", innerException);

public class ProductAlreadyInFavoritesException(Guid userId, Guid id)
    : UserException(userId,
        $"Product under id: {id} has already added to favorite products in user under id {id}");

public class ProductNotFoundException(Guid id)
    : UserException(Guid.Empty, $"Product under id: {id} not found");

public class UserFavoriteProductNotFoundException(Guid userId, Guid id)
    : UserException(userId, $"Product under id: {id} not found in favorite products in user under id: {id}");