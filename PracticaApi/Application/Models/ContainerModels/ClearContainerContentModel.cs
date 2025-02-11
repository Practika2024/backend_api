using Domain.Authentications.Users;
using Domain.Containers;

namespace Application.Models.ContainerModels;

public class ClearContainerContentModel
{
    public ContainerId ContainerId { get; set; }
    public UserId ModifiedBy { get; set; } 
}