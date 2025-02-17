using Domain.Containers;

namespace Api.Dtos.Containers
{
    public record ContainerDto
    {
        public Guid? Id { get; init; }
        public string Name { get; init; } = null!;
        public decimal Volume { get; init; }
        public string? Notes { get; init; }
        public string UniqueCode { get; init; }
        public Guid? TypeId { get; init; } = null!;
        public Guid? ContentId { get; init; } = null!;
        public DateTime CreatedAt { get; init; }
        public Guid CreatedBy { get; init; }
        public DateTime? ModifiedAt { get; init; }
        public Guid? ModifiedBy { get; init; }
        public static ContainerDto FromDomainModel(ContainerEntity containerEntity)
        {
            return new ContainerDto
            {
                Id = containerEntity.Id,
                Name = containerEntity.Name,
                Volume = containerEntity.Volume,
                Notes = containerEntity.Notes,
                UniqueCode = containerEntity.UniqueCode,
                TypeId = containerEntity.TypeId,
                ContentId = containerEntity.ContentId,
                CreatedAt = containerEntity.CreatedAt,
                CreatedBy = containerEntity.CreatedBy,
                ModifiedAt = containerEntity.ModifiedAt,
                ModifiedBy = containerEntity.ModifiedBy,
            };
        }
    }
}