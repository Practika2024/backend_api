using Application.Common;
using Application.Exceptions;
using Application.ViewModels;
using Domain.Containers;

namespace Application.Services.ContainerServices.DeleteContainerService;
public interface IDeleteContainerService
{
    Task<Result<ContainerVM, ContainerException>> DeleteContainerAsync(Guid containerId, CancellationToken cancellationToken);
}