namespace Domain.UserModels;

public class UpdateUserImageModel
{
    public Guid UserId { get; set; }
    public string? FilePath { get; set; } 
}