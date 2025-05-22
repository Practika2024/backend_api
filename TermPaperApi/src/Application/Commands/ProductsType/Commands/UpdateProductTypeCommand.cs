using System.Net;
using Application.Commands.ProductsType.Exceptions;
using Application.Common;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Repositories;
using Application.Services;
using Domain.ProductTypes;
using Domain.ProductTypes.Models;
using MediatR;

namespace Application.Commands.ProductsType.Commands;

public record UpdateProductTypeCommand : IRequest<ServiceResponse>
{
    public Guid Id { get; set; }
    public string Name { get; init; }
}

public class UpdateProductTypeCommandHandler(
    IProductTypeRepository productTypeRepository, IUserProvider userProvider)
    : IRequestHandler<UpdateProductTypeCommand, ServiceResponse>
{
    public async Task<ServiceResponse> Handle(
        UpdateProductTypeCommand request,
        CancellationToken cancellationToken)
    {
        var userId = userProvider.GetUserId();
        var productTypeId = request.Id;

        var existingProductType = await productTypeRepository.GetById(productTypeId, cancellationToken);

        return await existingProductType.Match(
            async product =>
            {
                try
                {
                    var updateProductModel = new UpdateProductTypeModel
                    {
                        Id = productTypeId,
                        Name = request.Name,
                        ModifiedBy = userId,
                    };
                    var updatedProduct = await productTypeRepository.Update(updateProductModel, cancellationToken);
                    return ServiceResponse.OkResponse("Product type updated", updatedProduct);
                }
                catch (ProductTypeException exception)
                {
                    return ServiceResponse.InternalServerErrorResponse(exception.Message);
                }
            },
            () => Task.FromResult<ServiceResponse>(
                ServiceResponse.NotFoundResponse("Product type not found", null))
        );
    }
}