using DataAccessLayer.Entities.ContainerHistories;
using DataAccessLayer.Entities.Users;
using Domain.Common.Abstractions;

namespace DataAccessLayer.Entities.Products;
public class ProductEntity : AuditableEntity<UserEntity>
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public DateTime ManufactureDate { get; set; }
    public Guid TypeId { get; set; }
    public ProductTypeEntity? Type { get; set; }
}