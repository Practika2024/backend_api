using Application.Commands.Containers.Exceptions;
using Application.Common;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Domain.Containers;
using Domain.Containers.Models;
using Domain.ContainersHistory.Models;
using MediatR;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Application.Commands.Containers.Commands;

public record SetContainerContentCommand : IRequest<Result<Container, ContainerException>>
{
    public required Guid ContainerId { get; init; }
    public required Guid? ProductId { get; init; }
}

public class SetContainerContentCommandHandler(
    IContainerRepository containerRepository,
    IContainerQueries containerQueries,
    IProductQueries productQueries,
    IContainerHistoryRepository containerHistoryRepository,
    IUserProvider userProvider)
    : IRequestHandler<SetContainerContentCommand, Result<Container, ContainerException>>
{
    public async Task<Result<Container, ContainerException>> Handle(
        SetContainerContentCommand request,
        CancellationToken cancellationToken)
    {
        var containerId = request.ContainerId;
        var existingContainer = await containerRepository.GetById(containerId, cancellationToken);
        var existingProduct = await productQueries.GetById(request.ProductId!.Value, cancellationToken);

        return await existingProduct.Match(
            async product =>
            {
                return await existingContainer.Match(
                    async container => await SetProduct(request, container, cancellationToken),
                    () => Task.FromResult<Result<Container, ContainerException>>(
                        new ContainerNotFoundException(containerId))
                );
            },
            () => Task.FromResult<Result<Container, ContainerException>>(
                new ProductNotFoundException(request.ProductId!.Value)));
    }

    private async Task<Result<Container, ContainerException>> SetProduct(SetContainerContentCommand request,
        Container container, CancellationToken cancellationToken)
    {
        if (await containerQueries.IsProductInContainer(request.ProductId!.Value, cancellationToken))
        {
            throw new Exception("Product already in container");
        }

        if (container.ProductId.HasValue)
        {
            throw new Exception("Container is not empty");
        }

        try
        {
            var userId = userProvider.GetUserId();
            var productId = request.ProductId != null
                ? request.ProductId
                : null;

            var setContainerContentModel = new SetContainerContentModel
            {
                ContainerId = container.Id,
                ProductId = productId,
                ModifiedBy = userId
            };

            var updatedContainer =
                await containerRepository.SetContainerContent(setContainerContentModel,
                    cancellationToken);

            await AddHistory(userId, container.Id, productId!.Value, cancellationToken);

            return updatedContainer;
        }
        catch (ContainerException exception)
        {
            return new ContainerUnknownException(container.Id, exception);
        }
    }

    private async Task AddHistory(Guid userId, Guid containerId, Guid productId, CancellationToken cancellationToken)
    {
        var historyModel = new CreateContainerHistoryModel
        {
            Id = Guid.NewGuid(),
            CreatedBy = userId,
            ContainerId = containerId,
            ProductId = productId,
            StartDate = DateTime.UtcNow,
        };

        await containerHistoryRepository.Create(historyModel, cancellationToken);
    }
}