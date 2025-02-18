using Domain.ContainerHistories;

namespace Api.Dtos.ContainerHistories;

public record ContainerHistoryDto
{
    public Guid Id { get; init; }
    public Guid ContainerId { get; init; }
    public Guid ProductId { get; init; }
    public DateTime StartDate { get; init; }
    public DateTime? EndDate { get; init; }
    public Guid CreatedBy { get; init; }

    public static ContainerHistoryDto FromDomainModel(ContainerHistoryEntity historyEntity)
    {
        return new ContainerHistoryDto
        {
            Id = historyEntity.Id,
            ContainerId = historyEntity.ContainerId,
            ProductId = historyEntity.ProductId,
            StartDate = historyEntity.StartDate,
            EndDate = historyEntity.EndDate,
            CreatedBy = historyEntity.CreatedBy
        };
    }
}