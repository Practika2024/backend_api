using Application.Common;
using Application.Exceptions;
using Application.ViewModels;
using Domain.Containers;
using Domain.Products;

namespace Application.Services.ContainerServices.SetCurrentProductService;
public interface ISetCurrentProductService
{
    Task<Result<ContainerVM, ContainerException>> SetCurrentProductAsync(
        Guid containerId,
        Guid productId,
        CancellationToken cancellationToken);
}