using Domain.Authentications.Users;

namespace Application.Authentications.Exceptions;

public abstract class AuthenticationException(UserId id, string message, Exception? innerException = null)
    : Exception(message, innerException)
{
    public UserId UserId { get; } = id;
}

public class UserByThisEmailAlreadyExistsAuthenticationException(UserId id) : AuthenticationException(id, $"User by this email already exists! User id: {id}");
public class EmailOrPasswordAreIncorrectException() : AuthenticationException(UserId.Empty, "Email or Password are incorrect!");

public class InvalidTokenException() : AuthenticationException(UserId.Empty, "Invalid token!");
public class TokenExpiredException() : AuthenticationException(UserId.Empty, "Token has expired!");
public class InvalidAccessTokenException() : AuthenticationException(UserId.Empty, "Token has expired!");
public class UserNorFoundException(UserId id) : AuthenticationException(id, $"User under id: {id} was not found!");

public class AuthenticationUnknownException(UserId id, Exception innerException)
    : AuthenticationException(id, $"Unknown exception for the user under id: {id}", innerException);