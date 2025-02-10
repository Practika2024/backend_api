using Domain.Containers;

namespace Application.ViewModels;
public class ContainerVM
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public decimal Volume { get; set; }
    public string? Notes { get; set; }
    public bool IsEmpty { get; set; }
    public string UniqueCode { get; set; }
    public ContainerType Type { get; set; }

    public ContainerVM(ContainerEntity containerEntity)
    {
        Id = containerEntity.Id.Value;
        Name = containerEntity.Name;
        Volume = containerEntity.Volume;
        Notes = containerEntity.Notes;
        IsEmpty = containerEntity.IsEmpty;
        UniqueCode = containerEntity.UniqueCode;
        Type = containerEntity.Type;
    }
}