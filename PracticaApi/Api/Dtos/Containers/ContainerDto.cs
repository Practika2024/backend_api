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
    }
}