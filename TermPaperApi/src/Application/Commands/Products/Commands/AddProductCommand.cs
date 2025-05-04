using System.Net;
using Application.Commands.Products.Exceptions;
using Application.Common;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Services;
using Domain.Products;
using Domain.Products.Models;
using Domain.ProductTypes;
using MediatR;

namespace Application.Commands.Products.Commands;

public record AddProductCommand : IRequest<ServiceResponse>
{
    public required string Name { get; init; }
    public required string? Description { get; init; }
    public required DateTime ManufactureDate { get; init; }
    public required Guid TypeId { get; init; }
}

public class AddProductCommandHandler(
    IProductRepository productRepository,
    IProductQueries productQueries,
    IProductTypeQueries productTypeQueries,
    IUserQueries userQueries, IUserProvider userProvider)
    : IRequestHandler<AddProductCommand, ServiceResponse>
{
    public async Task<ServiceResponse> Handle(
        AddProductCommand request,
        CancellationToken cancellationToken)
    {
        var existingProduct = await productQueries.SearchByName(request.Name, cancellationToken);

        return await existingProduct.Match<Task<ServiceResponse>>(
            c => Task.FromResult<ServiceResponse>(
                ServiceResponse.GetResponse("Product with this name already exists", false, null, HttpStatusCode.Conflict)),
            async () =>
            {
                var userResult = await userQueries.GetById(userProvider.GetUserId(), cancellationToken);
                return await userResult.Match<Task<ServiceResponse>>(
                    async user =>
                    {
                        var typeResult = await productTypeQueries.GetById(request.TypeId, cancellationToken);
                        return await typeResult.Match<Task<ServiceResponse>>(
                            async type =>
                            {
                                return await CreateEntity(
                                    request.Name,
                                    request.Description,
                                    request.ManufactureDate,
                                    type,
                                    cancellationToken);
                            },
                            () => Task.FromResult<ServiceResponse>(
                                ServiceResponse.NotFoundResponse("Product type not found"))
                        );
                    },
                    () => Task.FromResult<ServiceResponse>(
                        ServiceResponse.NotFoundResponse("User not found"))
                );
            });
    }

    private async Task<ServiceResponse> CreateEntity(
        string name,
        string? description,
        DateTime manufactureDate,
        ProductType type,
        CancellationToken cancellationToken)
    {
        try
        {
            var productId = Guid.NewGuid();
            var createProductModel = new CreateProductModel
            {
                Id = productId,
                Description = description,
                ManufactureDate = manufactureDate,
                Name = name,
                CreatedBy = userProvider.GetUserId(),
                TypeId = type.Id,
            };

            var createdProduct = await productRepository.Create(createProductModel, cancellationToken);
            return ServiceResponse.OkResponse("Product created", createdProduct);
        }
        catch (ProductException exception)
        {
            return ServiceResponse.InternalServerErrorResponse(exception.Message, exception);
        }
    }
}