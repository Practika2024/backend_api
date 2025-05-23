namespace Domain.Containers.Models;

public class CreateContainerModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public decimal Volume { get; set; }
    public string? Notes { get; set; }
    public Guid CreatedBy { get; set; }
    public Guid TypeId { get; set; }
    public string UniqueCode { get; set; }
}