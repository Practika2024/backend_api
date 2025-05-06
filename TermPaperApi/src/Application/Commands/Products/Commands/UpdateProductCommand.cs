using Application.Commands.Products.Exceptions;
using Application.Common;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Services;
using Domain.Products;
using Domain.Products.Models;
using MediatR;

namespace Application.Commands.Products.Commands;

public record UpdateProductCommand : IRequest<ServiceResponse>
{
    public Guid Id { get; set; }
    public string Name { get; init; }
    public Guid TypeId { get; init; }
    public string? Description { get; init; }
    public DateTime ManufactureDate { get; init; } 
}

public class UpdateProductCommandHandler(
    IProductRepository productRepository, IUserProvider userProvider, IProductTypeQueries productTypeQueries)
    : IRequestHandler<UpdateProductCommand, ServiceResponse>
{
    public async Task<ServiceResponse> Handle(
        UpdateProductCommand request,
        CancellationToken cancellationToken)
    {
        var userId = userProvider.GetUserId();
        var productId = request.Id;

        var existingProduct = await productRepository.GetById(productId, cancellationToken);
        var existingProductType = await productTypeQueries.GetById(request.TypeId, cancellationToken);

        return await existingProduct.Match(
            async product => await existingProductType.Match(
                async _ =>
                {
                    try
                    {
                        var updateProductModel = new UpdateProductModel
                        {
                            Id = productId,
                            Name = request.Name,
                            Description = request.Description,
                            ManufactureDate = request.ManufactureDate,
                            ModifiedBy = userId,
                            TypeId = request.TypeId,
                            Images = product.Images
                        };
                        var updatedProduct = await productRepository.Update(updateProductModel, cancellationToken);
                        return ServiceResponse.OkResponse("Product updated", updatedProduct);
                    }
                    catch (ProductException exception)
                    {
                        return ServiceResponse.InternalServerErrorResponse(exception.Message);
                    }
                },
                () => Task.FromResult<ServiceResponse>(ServiceResponse.NotFoundResponse("Product type not found"))
            ),
            () => Task.FromResult<ServiceResponse>(ServiceResponse.NotFoundResponse("Product not found"))
        );
    }
}
