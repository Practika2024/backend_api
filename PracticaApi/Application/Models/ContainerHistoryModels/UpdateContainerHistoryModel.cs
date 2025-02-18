using Domain.ContainerHistories;

namespace Application.Models.ContainerHistoryModels;
public class UpdateContainerHistoryModel
{
    public Guid Id { get; set; }
    public DateTime? EndDate { get; set; }
}