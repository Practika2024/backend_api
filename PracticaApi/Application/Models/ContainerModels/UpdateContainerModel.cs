using Domain.Authentications.Users;
using Domain.Containers;

namespace Application.Models.ContainerModels;


public class UpdateContainerModel
{
    public ContainerId Id { get; set; }
    public string Name { get; set; }
    public decimal Volume { get; set; }
    public string? Notes { get; set; }
    public UserId ModifiedBy { get; set; }
    public ContainerTypeId TypeId { get; set; }
    public string UniqueCode { get; set; }
}