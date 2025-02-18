using Domain.Abstractions;
using Domain.Containers;
using Domain.Products;
using Domain.Users;

namespace Domain.ContainerHistories;

public class ContainerHistoryEntity : AuditableEntity
{
    public Guid Id { get; }
    public Guid ContainerId { get; private set; }
    public ContainerEntity? Container { get; private set; }
    public Guid ProductId { get; private set; }
    public ProductEntity? Product { get; private set; }
    public DateTime StartDate { get; }
    public DateTime EndDate { get; private set; }
    public UserEntity? CreatedByNavigation { get; private set; }

    private ContainerHistoryEntity(
        Guid id,
        Guid containerId,
        Guid productId,
        DateTime startDate,
        Guid createdBy) : base(createdBy)
    {
        Id = id;
        ContainerId = containerId;
        ProductId = productId;
        StartDate = startDate;
    }

    public static ContainerHistoryEntity New(
        Guid id,
        Guid containerId,
        Guid productId,
        DateTime startDate,
        Guid createdBy)
        => new(id, containerId, productId, startDate, createdBy);

    public void SetEndDate(DateTime endDate)
        => EndDate = endDate;
}