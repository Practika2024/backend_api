﻿using Application.Commands.ProductsType.Exceptions;
using Application.Common;
using Application.Common.Interfaces.Repositories;
using Domain.ProductTypes;
using Domain.ProductTypes.Models;
using MediatR;

namespace Application.Commands.ProductsType.Commands;

public record AddProductTypeCommand : IRequest<Result<ProductType, ProductTypeException>>
{
    public required string Name { get; init; }
    public Guid CreatedBy { get; init; }
}

public class AddProductTypeCommandHandler(IProductTypeRepository productTypeRepository)
    : IRequestHandler<AddProductTypeCommand, Result<ProductType, ProductTypeException>>
{
    public async Task<Result<ProductType, ProductTypeException>> Handle(
        AddProductTypeCommand request,
        CancellationToken cancellationToken)
    {
        var existingProductType = await productTypeRepository.SearchByName(request.Name, cancellationToken);

        return await existingProductType.Match<Task<Result<ProductType, ProductTypeException>>>(
            c => throw new Exception("Product already exists"),
            async () =>
            {
                return await CreateEntity(request.Name, request.CreatedBy, cancellationToken);
            });
    }

    private async Task<Result<ProductType, ProductTypeException>> CreateEntity(
        string name,
        Guid createdBy,
        CancellationToken cancellationToken)
    {
        try
        {
            var productId = Guid.NewGuid();
            var createProductModel = new CreateProductTypeModel
            {
                Id = productId,
                Name = name,
                CreatedBy = createdBy
            };

            var createdProduct = await productTypeRepository.Create(createProductModel, cancellationToken);
            return createdProduct;
        }
        catch (ProductTypeException exception)
        {
            return new ProductUnknownException(Guid.Empty, exception);
        }
    }
}