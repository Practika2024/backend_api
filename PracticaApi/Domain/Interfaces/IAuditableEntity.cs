namespace Domain.Interfaces;

public interface IAuditableEntity
{
    Guid CreatedBy { get; }
    DateTime CreatedAt { get; }
    Guid ModifiedBy { get; }
    DateTime ModifiedAt { get; }
}