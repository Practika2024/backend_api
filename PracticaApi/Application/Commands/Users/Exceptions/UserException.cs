using Domain.Authentications.Users;
using Domain.Products;

namespace Application.Commands.Users.Exceptions;

public abstract class UserException(UserId id, string message, Exception innerException = null)
    : Exception(message, innerException)
{
    public UserId UserId { get; } = id;
}

public class UserByThisEmailAlreadyExistsException(UserId id)
    : UserException(id, $"User by this email already exists! User id: {id}");

public class EmailOrPasswordAreIncorrect() : UserException(UserId.Empty, "Email or Password are incorrect!");

public class UserNotFoundException(UserId id) : UserException(id, $"User under id: {id} was not found!");

public class ImageSaveException(UserId id) : UserException(id, $"User under id: {id} have problems with image save!");

public class RoleNotFoundException(string role)
    : UserException(UserId.Empty, $"Role under name: {role} was not found!");

public class UserUnknownException(UserId id, Exception innerException)
    : UserException(id, $"Unknown exception for the user under id: {id}", innerException);

public class ProductAlreadyInFavoritesException(UserId userId, ProductId id)
    : UserException(userId,
        $"Product under id: {id} has already added to favorite products in user under id {id}");

public class ProductNotFoundException(ProductId id)
    : UserException(UserId.Empty, $"Product under id: {id} not found");

public class UserFavoriteProductNotFoundException(UserId userId, ProductId id)
    : UserException(userId, $"Product under id: {id} not found in favorite products in user under id: {id}");