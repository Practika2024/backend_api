using Domain.ContainerHistories;

namespace Application.ViewModels;
public class ContainerHistoryVM
{
    public Guid Id { get; set; }
    public Guid ContainerId { get; set; }
    public Guid ProductId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    public ContainerHistoryVM(ContainerHistory history)
    {
        Id = history.Id.Value;
        ContainerId = history.ContainerId.Value;
        ProductId = history.ProductId.Value;
        StartDate = history.StartDate;
        EndDate = history.EndDate;
    }
}