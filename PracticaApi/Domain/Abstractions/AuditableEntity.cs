using Domain.Interfaces;
using Domain.Users;

namespace Domain.Abstractions;

public class AuditableEntity : IAuditableEntity
{
    public virtual Guid CreatedBy { get; set; }
    public virtual UserEntity? CreatedByEntity { get; set; }
    public virtual DateTime CreatedAt { get; set; } 
    public virtual Guid ModifiedBy { get; set; }
    public virtual DateTime ModifiedAt { get; set; }
}