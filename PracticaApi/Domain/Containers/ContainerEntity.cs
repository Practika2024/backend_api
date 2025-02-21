using Domain.Abstractions;
using Domain.ContainerHistories;
using Domain.Interfaces;
using Domain.Products;
using Domain.Users;

namespace Domain.Containers;

internal class ContainerEntity : AuditableEntity
{
    public Guid Id { get; }
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