using Application.Commands.Products.Exceptions;
using Application.Common;
using Application.Common.Interfaces.Repositories;
using Domain.Products;
using Domain.Products.Models;
using MediatR;

namespace Application.Commands.Products.Commands;

public record DeleteProductCommand : IRequest<Result<Product, ProductException>>
{
    public required Guid Id { get; init; }
}

public class DeleteProductCommandHandler(
    IProductRepository productRepository)
    : IRequestHandler<DeleteProductCommand, Result<Product, ProductException>>
{
    public async Task<Result<Product, ProductException>> Handle(
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
                    return deletedProduct;
                }
                catch (ProductException exception)
                {
                    return new ProductUnknownException(productIdObj, exception);
                }
            },
            () => Task.FromResult<Result<Product, ProductException>>(
                new ProductNotFoundException(productIdObj))
        );
    }
}