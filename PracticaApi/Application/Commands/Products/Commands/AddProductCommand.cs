using Application.Commands.Products.Exceptions;
using Application.Common;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Domain.Products;
using Domain.Products.Models;
using Domain.ProductTypes;
using MediatR;

namespace Application.Commands.Products.Commands;

public record AddProductCommand : IRequest<Result<Product, ProductException>>
{
    public required string Name { get; init; }
    public required string? Description { get; init; }
    public required DateTime ManufactureDate { get; init; }
    public required Guid UserId { get; init; }
    public required Guid TypeId { get; init; }
}

public class AddProductCommandHandler(
    IProductRepository productRepository,
    IProductQueries productQueries,
    IProductTypeQueries productTypeQueries,
    IUserQueries userQueries)
    : IRequestHandler<AddProductCommand, Result<Product, ProductException>>
{
    public async Task<Result<Product, ProductException>> Handle(
        AddProductCommand request,
        CancellationToken cancellationToken)
    {
        var existingProduct = await productQueries.SearchByName(request.Name, cancellationToken);

        return await existingProduct.Match<Task<Result<Product, ProductException>>>(
            c => Task.FromResult<Result<Product, ProductException>>(
                new ProductAlreadyExistsException(c.Id)),
            async () =>
            {
                var userResult = await userQueries.GetById(request.UserId, cancellationToken);
                return await userResult.Match<Task<Result<Product, ProductException>>>(
                    async user =>
                    {
                        var typeResult = await productTypeQueries.GetById(request.TypeId, cancellationToken);
                        return await typeResult.Match<Task<Result<Product, ProductException>>>(
                            async type =>
                            {
                                return await CreateEntity(
                                    request.Name,
                                    request.Description,
                                    request.ManufactureDate,
                                    request.UserId,
                                    type,
                                    cancellationToken);
                            },
                            () => Task.FromResult<Result<Product, ProductException>>(
                                new ProductTypeNotFoundException(request.TypeId))
                        );
                    },
                    () => Task.FromResult<Result<Product, ProductException>>(
                        new UserNotFoundException(request.UserId))
                );
            });
    }

    private async Task<Result<Product, ProductException>> CreateEntity(
        string name,
        string? description,
        DateTime manufactureDate,
        Guid userId,
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
                CreatedBy = userId,
                TypeId = type.Id,
            };

            var createdProduct = await productRepository.Create(createProductModel, cancellationToken);
            return createdProduct;
        }
        catch (ProductException exception)
        {
            return new ProductUnknownException(Guid.Empty, exception);
        }
    }
}