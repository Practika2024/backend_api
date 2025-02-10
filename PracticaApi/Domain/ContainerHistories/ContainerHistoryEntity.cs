using Domain.Authentications.Users;
using Domain.Containers;
using Domain.Products;

namespace Domain.ContainerHistories;

public class ContainerHistoryEntity
{
    public ContainerHistoryId Id { get; }
    public ContainerId ContainerId { get; private set; }
    public ContainerEntity? Container { get; private set; }
    public ProductId ProductId { get; private set; } 
    public ProductEntity? Product { get; private set; }
    public DateTime StartDate { get; }
    public DateTime? EndDate { get; private set; }
    public UserId CreatedBy { get; private set; }
    public UserEntity? CreatedByNavigation { get; private set; }

    private ContainerHistoryEntity(
        ContainerHistoryId id,
        ContainerId containerId,
        ProductId productId,
        DateTime startDate,
        UserId createdBy)
    {
        Id = id;
        ContainerId = containerId;
        ProductId = productId;
        StartDate = startDate;
        CreatedBy = createdBy;
    }

    public static ContainerHistoryEntity New(
        ContainerHistoryId id,
        ContainerId containerId,
        ProductId productId,
        DateTime startDate,
        UserId createdBy)
        => new(id, containerId, productId, startDate, createdBy);

    public void SetEndDate(DateTime endDate)
        => EndDate = endDate;


}

public record ContainerHistoryId(Guid Value)
{
    public static ContainerHistoryId New() => new(Guid.NewGuid());
    public static ContainerHistoryId Empty => new(Guid.Empty);
    public override string ToString() => Value.ToString();
}