namespace Api.Dtos.Users;

public record CreateUserDto
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string? Name { get; set; }
    public string? Surname { get; set; }
    public string? Patronymic { get; set; }
}