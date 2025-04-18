﻿using Application.Commands.ProductsType.Exceptions;
using Application.Common;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Repositories;
using Domain.ProductTypes;
using Domain.ProductTypes.Models;
using MediatR;

namespace Application.Commands.ProductsType.Commands;

public record UpdateProductTypeCommand : IRequest<Result<ProductType, ProductTypeException>>
{
    public Guid Id { get; set; }
    public string Name { get; init; }
}

public class UpdateProductTypeCommandHandler(
    IProductTypeRepository productTypeRepository, IUserProvider userProvider)
    : IRequestHandler<UpdateProductTypeCommand, Result<ProductType, ProductTypeException>>
{
    public async Task<Result<ProductType, ProductTypeException>> Handle(
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
                    return updatedProduct;
                }
                catch (ProductTypeException exception)
                {
                    return new ProductUnknownException(productTypeId, exception);
                }
            },
            () => Task.FromResult<Result<ProductType, ProductTypeException>>(
                new ProductTypeNotFoundException(productTypeId))
        );
    }
}