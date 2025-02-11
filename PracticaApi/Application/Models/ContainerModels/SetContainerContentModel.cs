using Domain.Authentications.Users;
using Domain.Containers;
using Domain.Products;

namespace Application.Models.ContainerModels;

public class SetContainerContentModel
{
    public ContainerId ContainerId { get; set; }
    public ProductId? ProductId { get; set; } 
    public bool IsEmpty { get; set; } 
    public UserId ModifiedBy { get; set; } 
}