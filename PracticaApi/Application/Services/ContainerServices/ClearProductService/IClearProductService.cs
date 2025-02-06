using Application.Common;
using Application.Exceptions;
using Application.ViewModels;
using Domain.Containers;

namespace Application.Services.ContainerServices.ClearProductService;
public interface IClearProductService
{
    Task<Result<ContainerVM, ContainerException>> ClearProductAsync(
        Guid containerId,
        CancellationToken cancellationToken);
}