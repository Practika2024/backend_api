using Domain.Interfaces;
using Domain.Users;

namespace Domain.Abstractions;

public abstract class AuditableEntity(Guid createdBy) : IAuditableEntity
{
    public virtual Guid CreatedBy { get; set; } = createdBy;
    public virtual UserEntity? CreatedByEntity { get; set; }
    public virtual DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public virtual Guid ModifiedBy { get; set; } = createdBy;
    public virtual DateTime ModifiedAt { get; set; } = DateTime.UtcNow;

    protected virtual void Update(Guid modifiedBy)
    {
        ModifiedBy = modifiedBy;
        ModifiedAt = DateTime.UtcNow;
    }
}