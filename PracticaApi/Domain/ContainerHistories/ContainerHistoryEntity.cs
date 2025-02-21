using Domain.Abstractions;
using Domain.Containers;
using Domain.Products;
using Domain.Users;

namespace Domain.ContainerHistories;

internal class ContainerHistoryEntity : AuditableEntity
{
    public Guid Id { get; }
    public Guid ContainerId { get; set; }
    public ContainerEntity? Container { get; set; }
    public Guid ProductId { get; set; }
    public ProductEntity? Product { get; set; }
    public DateTime StartDate { get; }
    public DateTime EndDate { get; set; }
}