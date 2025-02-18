using Domain.Containers;

namespace Application.Models.ContainerModels;

public class ClearContainerContentModel
{
    public Guid ContainerId { get; set; }
    public Guid ModifiedBy { get; set; } 
}