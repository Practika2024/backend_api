using DataAccessLayer.Entities.Containers;
using DataAccessLayer.Entities.Products;
using DataAccessLayer.Entities.Users;
using Domain.Common.Abstractions;

namespace DataAccessLayer.Entities.ContainerHistories;

public class ContainerHistoryEntity : AuditableEntity<UserEntity>
{
    public Guid Id { get; set; }
    public Guid ContainerId { get; set; }
    public ContainerEntity? Container { get; set; }
    public Guid? ProductId { get; set; }
    public ProductEntity? Product { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}