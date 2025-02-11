namespace Application.Dtos.Containers;

public class SetContainerContentDto
{
    public Guid? ProductId { get; set; }
    public bool IsEmpty { get; set; } 
    public Guid ModifiedBy { get; set; }
}