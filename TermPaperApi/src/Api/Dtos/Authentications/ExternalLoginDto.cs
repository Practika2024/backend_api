namespace Api.Dtos.Authentications;

public class ExternalLoginDto
{
    public required string Provider { get; set; }
    public required string Token { get; set; }
}