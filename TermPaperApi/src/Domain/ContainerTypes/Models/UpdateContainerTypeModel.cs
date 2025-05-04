namespace Domain.ContainerTypes.Models;


public class UpdateContainerTypeModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public Guid ModifiedBy { get; set; }
}