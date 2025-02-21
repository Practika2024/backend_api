namespace Api.Dtos.Authentications;

public record SignUpDto
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string? Name { get; set; }
    public string? Surname { get; set; }
    public string? Patronymic { get; set; }
}