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

    public ContainerVM(Container container)
    {
        Id = container.Id.Value;
        Name = container.Name;
        Volume = container.Volume;
        Notes = container.Notes;
        IsEmpty = container.IsEmpty;
        UniqueCode = container.UniqueCode;
        Type = container.Type;
    }
}