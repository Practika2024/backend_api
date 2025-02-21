namespace Api.Dtos.Authentications;

public record SignInDto
{
    public string Email { get; set; }
    public string Password { get; set; }
}