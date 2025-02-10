using Domain.ContainerHistories;

namespace Application.ViewModels;
public class ContainerHistoryVM
{
    public Guid Id { get; set; }
    public Guid ContainerId { get; set; }
    public Guid ProductId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    public ContainerHistoryVM(ContainerHistoryEntity historyEntity)
    {
        Id = historyEntity.Id.Value;
        ContainerId = historyEntity.ContainerId.Value;
        ProductId = historyEntity.ProductId.Value;
        StartDate = historyEntity.StartDate;
        EndDate = historyEntity.EndDate;
    }
}