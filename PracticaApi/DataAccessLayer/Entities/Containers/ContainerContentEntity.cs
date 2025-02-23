using DataAccessLayer.Entities.Products;
using DataAccessLayer.Entities.Users;
using Domain.Common.Abstractions;

namespace DataAccessLayer.Entities.Containers;

public class ContainerContentEntity : AuditableEntity<UserEntity>
{
    public Guid Id { get; set; }
    public Guid? ProductId { get; set; }
    public ProductEntity? Product { get; set; }
    public bool IsEmpty { get; set; }
}