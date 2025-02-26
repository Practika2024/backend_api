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
        public Guid? ProductId { get; init; } = null!;
        public bool IsEmpty => !ProductId.HasValue;
    }
}