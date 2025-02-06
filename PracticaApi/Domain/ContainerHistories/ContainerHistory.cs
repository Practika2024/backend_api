using Domain.Authentications.Users;
using Domain.Containers;
using Domain.Products;

namespace Domain.ContainerHistories;

public class ContainerHistory
{
    public ContainerHistoryId Id { get; }
    public ContainerId ContainerId { get; private set; }
    public Container? Container { get; private set; }
    public ProductId ProductId { get; private set; } 
    public Product? Product { get; private set; }
    public DateTime StartDate { get; }
    public DateTime? EndDate { get; private set; }
    public UserId CreatedBy { get; private set; }
    public User? CreatedByNavigation { get; private set; }

    private ContainerHistory(
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

    public static ContainerHistory New(
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