using Domain.Containers;

namespace Api.ViewModels.Containers;
public class UpdateContainerVM
{
    public required string Name { get; init; }
    public required decimal Volume { get; init; }
    public required Guid UserId { get; init; }
    public string? Notes { get; init; }
    public required ContainerType Type { get; init; }
}