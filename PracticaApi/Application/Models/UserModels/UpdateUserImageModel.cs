using Domain.Authentications.Users;

namespace Application.Models.UserModels;

public class UpdateUserImageModel
{
    public UserId UserId { get; set; }
    public string? FilePath { get; set; } 
}