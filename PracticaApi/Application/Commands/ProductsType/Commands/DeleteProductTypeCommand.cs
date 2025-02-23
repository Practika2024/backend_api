using Application.Commands.ProductsType.Exceptions;
using Application.Common;
using Application.Common.Interfaces.Repositories;
using Domain.ProductTypeModels;
using MediatR;

namespace Application.Commands.ProductsType.Commands;

public record DeleteProductTypeCommand : IRequest<Result<ProductType, ProductTypeException>>
{
    public required Guid Id { get; init; }
}

public class DeleteProductTypeCommandHandler(
    IProductTypeRepository productTypeRepository)
    : IRequestHandler<DeleteProductTypeCommand, Result<ProductType, ProductTypeException>>
{
    public async Task<Result<ProductType, ProductTypeException>> Handle(
        DeleteProductTypeCommand request,
        CancellationToken cancellationToken)
    {
        var productTypeIdObj = request.Id;
        var existingProductType = await productTypeRepository.GetById(productTypeIdObj, cancellationToken);
        return await existingProductType.Match(
            async productType =>
            {
                try
                {
                    var deletedProductType = await productTypeRepository.Delete(productType.Id, cancellationToken);
                    return deletedProductType;
                }
                catch (ProductTypeException exception)
                {
                    return new ProductUnknownException(productTypeIdObj, exception);
                }
            },
            () => Task.FromResult<Result<ProductType, ProductTypeException>>(
                new ProductTypeNotFoundException(productTypeIdObj))
        );
    }
}