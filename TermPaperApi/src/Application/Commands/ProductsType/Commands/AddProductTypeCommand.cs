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

public record AddProductTypeCommand : IRequest<ServiceResponse>
{
    public required string Name { get; init; }
}

public class AddProductTypeCommandHandler(IProductTypeRepository productTypeRepository, IUserProvider userProvider)
    : IRequestHandler<AddProductTypeCommand, ServiceResponse>
{
    public async Task<ServiceResponse> Handle(
        AddProductTypeCommand request,
        CancellationToken cancellationToken)
    {
        var existingProductType = await productTypeRepository.SearchByName(request.Name, cancellationToken);

        return await existingProductType.Match<Task<ServiceResponse>>(
            c => Task.FromResult<ServiceResponse>(
                ServiceResponse.GetResponse("Product type with this name already exists", false, null, HttpStatusCode.Conflict)),
            async () => { return await CreateEntity(request.Name, userProvider.GetUserId(), cancellationToken); });
    }

    private async Task<ServiceResponse> CreateEntity(
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
            return ServiceResponse.OkResponse("Product type", createdProduct);
        }
        catch (Exception exception)
        {
            return ServiceResponse.InternalServerErrorResponse(exception.Message);
        }
    }
}