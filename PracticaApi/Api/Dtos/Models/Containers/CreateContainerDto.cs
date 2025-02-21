namespace Api.Dtos.Containers
{
    public class CreateContainerDto
    {
        public string Name { get; set; } = null!;
        public decimal Volume { get; set; }
        public string? Notes { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid TypeId { get; set; }
    }
}