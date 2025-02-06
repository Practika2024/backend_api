using Application.Common;
using Application.Exceptions;
using Application.ViewModels;
using Domain.Authentications.Users;
using Domain.Containers;

namespace Application.Services.ContainerServices.UpdateContainerService;
public interface IUpdateContainerService
{
    Task<Result<ContainerVM, ContainerException>> UpdateContainerAsync(
        Guid containerId,
        string name,
        decimal volume,        
        UserId userId,
        string? notes,
        ContainerType type,
        CancellationToken cancellationToken);
}