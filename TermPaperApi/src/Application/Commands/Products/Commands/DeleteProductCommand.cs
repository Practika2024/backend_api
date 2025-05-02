using Application.Commands.Products.Exceptions;
using Application.Common;
using Application.Common.Interfaces.Repositories;
using Application.Services;
using Domain.Products;
using Domain.Products.Models;
using MediatR;

namespace Application.Commands.Products.Commands;

public record DeleteProductCommand : IRequest<ServiceResponse>
{
    public required Guid Id { get; init; }
}

public class DeleteProductCommandHandler(
    IProductRepository productRepository)
    : IRequestHandler<DeleteProductCommand, ServiceResponse>
{
    public async Task<ServiceResponse> Handle(
        DeleteProductCommand request,
        CancellationToken cancellationToken)
    {
        var productIdObj = request.Id;
        var existingProduct = await productRepository.GetById(productIdObj, cancellationToken);
        return await existingProduct.Match(
            async product =>
            {
                try
                {
                    var model = new DeleteProductModel
                    {
                        Id = product.Id
                    };
                    var deletedProduct = await productRepository.Delete(model, cancellationToken);
                    return ServiceResponse.OkResponse("Product deleted", deletedProduct);
                }
                catch (ProductException exception)
                {
                    return ServiceResponse.InternalServerErrorResponse(exception.Message, exception);
                }
            },
            () => Task.FromResult<ServiceResponse>(
                ServiceResponse.NotFoundResponse("Product not found"))
        );
    }
}