using Application.Commands.ProductsType.Exceptions;
using Application.Common;
using Application.Common.Interfaces.Repositories;
using Application.Services;
using Domain.ProductTypes;
using MediatR;

namespace Application.Commands.ProductsType.Commands;

public record DeleteProductTypeCommand : IRequest<ServiceResponse>
{
    public required Guid Id { get; init; }
}

public class DeleteProductTypeCommandHandler(
    IProductTypeRepository productTypeRepository)
    : IRequestHandler<DeleteProductTypeCommand, ServiceResponse>
{
    public async Task<ServiceResponse> Handle(
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
                    return ServiceResponse.OkResponse("Product type deleted", deletedProductType);
                }
                catch (ProductTypeException exception)
                {
                    return ServiceResponse.InternalServerErrorResponse(exception.Message, exception);
                }
            },
            () => Task.FromResult<ServiceResponse>(
                ServiceResponse.NotFoundResponse("Product type not found"))
        );
    }
}