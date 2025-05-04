using Domain.Common.Interfaces;

namespace Domain.Common.Abstractions;

public class AuditableEntity : IAuditableEntity
{
    public Guid? CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } 
    public Guid? ModifiedBy { get; set; }
    public DateTime ModifiedAt { get; set; }
}