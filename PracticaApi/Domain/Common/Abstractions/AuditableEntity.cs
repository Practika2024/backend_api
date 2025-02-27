using Domain.Common.Interfaces;

namespace Domain.Common.Abstractions;

public class AuditableEntity<T> : IAuditableEntity
{
    public Guid CreatedBy { get; set; }
    public T? CreatedByEntity { get; set; }
    public DateTime CreatedAt { get; set; } 
    public Guid ModifiedBy { get; set; }
    public DateTime ModifiedAt { get; set; }
}