using Domain.Containers;

namespace Application.ViewModels;
public class ContainerVM
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public decimal Volume { get; set; }
    public string? Notes { get; set; }
    public string UniqueCode { get; set; }
    public ContainerTypeId TypeId { get; set; }

    public ContainerVM(ContainerEntity containerEntity)
    {
        Id = containerEntity.Id.Value;
        Name = containerEntity.Name;
        Volume = containerEntity.Volume;
        Notes = containerEntity.Notes;
        UniqueCode = containerEntity.UniqueCode;
        TypeId = containerEntity.Type.Id;
    }
}