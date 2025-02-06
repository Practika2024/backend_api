using Application.Common;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Exceptions;
using Application.ViewModels;
using Domain.Authentications.Users;
using Domain.Containers;
using Domain.Products;
using Optional;

namespace Application.Services.ContainerServices.SetCurrentProductService;
public class SetCurrentProductService : ISetCurrentProductService
{
    private readonly IContainerRepository _containerRepository;
    private readonly IProductQueries _productQueries;

    public SetCurrentProductService(
        IContainerRepository containerRepository,
        IProductQueries productQueries)
    {
        _containerRepository = containerRepository;
        _productQueries = productQueries;
    }

    public async Task<Result<ContainerVM, ContainerException>> SetCurrentProductAsync(
        Guid containerId,
        Guid productId,
        CancellationToken cancellationToken)
    {
        var containerIdObj = new ContainerId(containerId);
        var productIdObj = new ProductId(productId);

        var container = await _containerRepository.GetById(containerIdObj, cancellationToken);
        var product = await _productQueries.GetById(productIdObj, cancellationToken);

        return await container.Match<Task<Result<ContainerVM, ContainerException>>>(
            async c =>
            {
                return await product.Match<Task<Result<ContainerVM, ContainerException>>>(
                    async p =>
                    {
                        c.SetCurrentProduct(p, UserId.Empty);
                        var updatedContainer = await _containerRepository.Update(c, cancellationToken);
                        return new ContainerVM(updatedContainer);
                    },
                    () => Task.FromResult<Result<ContainerVM, ContainerException>>(new ProductForContainerNotFoundException(containerIdObj))
                );
            },
            () => Task.FromResult<Result<ContainerVM, ContainerException>>(new ContainerNotFoundException(containerIdObj))
        );
    }
}