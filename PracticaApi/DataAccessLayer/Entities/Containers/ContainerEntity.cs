using DataAccessLayer.Entities.ContainerHistories;
using DataAccessLayer.Entities.Users;
using Domain.Common.Abstractions;

namespace DataAccessLayer.Entities.Containers;

public class ContainerEntity : AuditableEntity<UserEntity>
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public decimal Volume { get; set; }
    public string? Notes { get; set; }
    public ICollection<ContainerHistoryEntity> Histories { get; set; } = new List<ContainerHistoryEntity>();
    public string UniqueCode { get; set; }
    public Guid TypeId { get; set; }
    public ContainerTypeEntity? Type { get; set; }
    public Guid? ContentId { get; set; }
    public ContainerContentEntity? Content { get; set; }
}