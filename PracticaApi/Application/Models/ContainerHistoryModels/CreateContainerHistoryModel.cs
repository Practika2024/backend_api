using Domain.Authentications.Users;
using Domain.ContainerHistories;
using Domain.Containers;
using Domain.Products;

namespace Application.Models.ContainerHistoryModels;
public class CreateContainerHistoryModel
{
    public ContainerHistoryId Id { get; set; }
    public ContainerId ContainerId { get; set; }
    public ProductId ProductId { get; set; }
    public DateTime StartDate { get; set; }
    public UserId CreatedBy { get; set; }
}