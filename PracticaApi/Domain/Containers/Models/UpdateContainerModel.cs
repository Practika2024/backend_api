namespace Domain.Containers.Models;


public class UpdateContainerModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public decimal Volume { get; set; }
    public string? Notes { get; set; }
    public Guid ModifiedBy { get; set; }
    public Guid TypeId { get; set; }
    public string UniqueCode { get; set; }
}