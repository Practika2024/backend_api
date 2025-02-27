namespace Domain.ContainersHistory.Models;
public class CreateContainerHistoryModel
{
    public Guid Id { get; set; }
    public Guid ContainerId { get; set; }
    public Guid ProductId { get; set; }
    public DateTime StartDate { get; set; }
    public Guid CreatedBy { get; set; }
}