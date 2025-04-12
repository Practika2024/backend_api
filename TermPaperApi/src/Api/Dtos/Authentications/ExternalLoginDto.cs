namespace Api.Dtos.Authentications;

public class ExternalLoginDto
{
    public string Provider { get; set; }
    public string Token { get; set; }
}