namespace Application.Commands.Authentications.Exceptions;

public abstract class AuthenticationException(Guid id, string message, Exception? innerException = null)
    : Exception(message, innerException)
{
    public Guid UserId { get; } = id;
}

public class UserByThisEmailAlreadyExistsAuthenticationException(Guid id) : AuthenticationException(id, $"User by this email already exists! User id: {id}");
public class EmailOrPasswordAreIncorrectException() : AuthenticationException(Guid.Empty, "Email or Password are incorrect!");

public class InvalidTokenException() : AuthenticationException(Guid.Empty, "Invalid token!");
public class TokenExpiredException() : AuthenticationException(Guid.Empty, "Token has expired!");
public class InvalidAccessTokenException() : AuthenticationException(Guid.Empty, "Token has expired!");
public class UserNorFoundException(Guid id) : AuthenticationException(id, $"User under id: {id} was not found!");

public class AuthenticationUnknownException(Guid id, Exception innerException)
    : AuthenticationException(id, $"Unknown exception for the user under id: {id}", innerException);
    
public class MissingGoogleTokenException() 
    : AuthenticationException(Guid.Empty, "Токен Google не надіслано.");

public class InvalidGoogleTokenException() 
    : AuthenticationException(Guid.Empty, "Недійсний токен Google.");

public class FailedToAddGoogleLoginException() 
    : AuthenticationException(Guid.Empty, "Не вдалося додати вход через Google.");