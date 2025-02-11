using Domain.ContainerHistories;

namespace Application.Models.ContainerHistoryModels;
public class UpdateContainerHistoryModel
{
    public ContainerHistoryId Id { get; set; }
    public DateTime? EndDate { get; set; }
}