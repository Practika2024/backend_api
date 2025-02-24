using Application.Commands.Containers.Exceptions;
using Application.Common;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Domain.ContainerHistoryModels;
using Domain.ContainerModels;
using Domain.ProductModels;
using MediatR;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Application.Commands.Containers.Commands;

public record SetContainerContentCommand : IRequest<Result<Container, ContainerException>>
{
    public required Guid ContainerId { get; init; }
    public required Guid? ProductId { get; init; }
    public required Guid ModifiedBy { get; init; }
}

public class SetContainerContentCommandHandler(
    IContainerRepository containerRepository,
    IContainerQueries containerQueries,
    IProductQueries productQueries,
    IContainerHistoryRepository containerHistoryRepository)
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
                    async c => await SetProduct(request, containerId, cancellationToken),
                    () => Task.FromResult<Result<Container, ContainerException>>(
                        new ContainerNotFoundException(containerId))
                );
            },
            () => Task.FromResult<Result<Container, ContainerException>>(
                new ProductNotFoundException(request.ProductId!.Value)));
    }

    private async Task<Result<Container, ContainerException>> SetProduct(SetContainerContentCommand request,
        Guid containerId, CancellationToken cancellationToken)
    {
        if (await containerQueries.IsProductInContainer(request.ProductId!.Value, cancellationToken))
        {
            throw new Exception("Product already in container");
        }

        try
        {
            var userId = request.ModifiedBy;
            var productId = request.ProductId != null
                ? request.ProductId
                : null;

            var setContainerContentModel = new SetContainerContentModel
            {
                ContainerId = containerId,
                ProductId = productId,
                ModifiedBy = userId
            };

            var updatedContainer =
                await containerRepository.SetContainerContent(setContainerContentModel,
                    cancellationToken);

            await AddHistory(userId, containerId, productId!.Value, cancellationToken);

            return updatedContainer;
        }
        catch (ContainerException exception)
        {
            return new ContainerUnknownException(containerId, exception);
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