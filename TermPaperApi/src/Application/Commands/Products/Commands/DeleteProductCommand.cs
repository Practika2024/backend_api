using System.Net;
using Application.Commands.Products.Exceptions;
using Application.Common;
using Application.Common.Interfaces.Repositories;
using Application.Services;
using Application.Services.ImageService;
using Application.Settings;
using Domain.Products;
using Domain.Products.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Application.Commands.Products.Commands;

public record DeleteProductCommand : IRequest<ServiceResponse>
{
    public required Guid Id { get; init; }
}

public class DeleteProductCommandHandler(
    IProductRepository productRepository, IImageService imageService)
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
                    
                    DeleteImageByUser(product.Images);
                    
                    return ServiceResponse.OkResponse("Product deleted");
                }
                catch (DbUpdateException ex) when (ex.InnerException is PostgresException pgEx && pgEx.SqlState == "23503")
                {
                    return ServiceResponse.GetResponse("Product has dependencies", false, null, HttpStatusCode.Conflict);
                }
                catch (Exception exception)
                {
                    return ServiceResponse.InternalServerErrorResponse(exception.Message, exception);
                }
            },
            () => Task.FromResult(
                ServiceResponse.NotFoundResponse("Product not found"))
        );
    }

    private void DeleteImageByUser(List<ProductImage> productImages)
    {
        foreach (var productImage in productImages)
        {
            imageService.DeleteImageAsync(ImagePaths.ProductImagesPath, productImage.FileName);
        }
    }
}