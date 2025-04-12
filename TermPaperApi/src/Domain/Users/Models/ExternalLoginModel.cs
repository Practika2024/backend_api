namespace Domain.Users.Models;

public class ExternalLoginModel
{
    public string Provider { get; set; }
    public string Token { get; set; }
}