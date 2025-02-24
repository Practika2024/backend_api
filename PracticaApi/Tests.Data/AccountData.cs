using Api.Dtos.Authentications;
using Domain.UserModels;

namespace Tests.Data;

public static class AccountData
{
    public static SignUpDto SignUpRequest => new()
    {
        Email = "testuser@example.com",
        Password = "StrongPassword123!",
        Name = "Test User"
        
    };

    public static SignInDto SignInRequest => new()
    {
        Email = "loginedtestuser@example.com",
        Password = "StrongPassword123!"
    };

    public static SignUpDto SignUpWithOutPasswordRequest => new()
    {
        Email = "testuser@example.com",
        Password = null,
        Name = "Test User"
    };

    public static SignUpDto SignUpWithInvalidEmailRequest => new()
    {
        Email = "invalid-email",
        Password = "Password123!",
        Name = "Test User"
    };

    public static SignUpDto SignUpWithoutNameRequest => new()
    {
        Email = "testuser@example.com",
        Password = "Password123!",
        Name = string.Empty
    };

    public static SignUpDto SignUpForSignInRequest => new()
    {
        Email = "loginedtestuser@example.com",
        Password = "StrongPassword123!",
        Name = "Test User"
    };

    public static SignInDto SignInWithInvalidCredentialsRequest => new()
    {
        Email = "loginedtestuser@example.com",
        Password = "WrongPassword123!"
    };

    public static SignInDto SignInWithInvalidEmailRequest => new()
    {
        Email = "invalid-email",
        Password = "Password123!"
    };

    public static SignInDto SignInWithoutPasswordRequest => new()
    {
        Email = "loginedtestuser@example.com",
        Password = null
    };

}