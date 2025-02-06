using Application.ViewModels;
using Domain.Containers;

namespace Api.Dtos.Containers;
public class ContainerDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public decimal Volume { get; set; }
    public string? Notes { get; set; }
    public bool IsEmpty { get; set; }
    public string UniqueCode { get; set; }
    public ContainerType Type { get; set; }

    public static ContainerDto FromDomainModel(Container container)
    {
        return new ContainerDto
        {
            Id = container.Id.Value,
            Name = container.Name,
            Volume = container.Volume,
            Notes = container.Notes,
            IsEmpty = container.IsEmpty,
            UniqueCode = container.UniqueCode,
            Type = container.Type
        };
    }
    public static ContainerDto FromDomainModel(ContainerVM container)
    {
        return new ContainerDto
        {
            Id = container.Id,
            Name = container.Name,
            Volume = container.Volume,
            Notes = container.Notes,
            IsEmpty = container.IsEmpty,
            UniqueCode = container.UniqueCode,
            Type = container.Type
        };
    }
}