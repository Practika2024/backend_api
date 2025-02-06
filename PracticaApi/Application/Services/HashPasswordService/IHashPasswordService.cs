namespace Application.Services.HashPasswordService;

public interface IHashPasswordService
{
    string HashPassword(string password);
    bool VerifyPassword(string password, string storedHash);
}