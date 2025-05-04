namespace Domain.ContainerTypes.Models;

public class CreateContainerTypeModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public Guid CreatedBy { get; set; }
}