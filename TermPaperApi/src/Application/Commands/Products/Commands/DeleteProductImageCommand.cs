using Application.Common.Interfaces.Repositories;
using Application.Services;
using Application.Services.ImageService;
using Application.Settings;
using AutoMapper;
using Domain.Products;
using Domain.Products.Models;
using MediatR;

namespace Application.Commands.Products.Commands;

public record DeleteProductImageCommand : IRequest<ServiceResponse>
{
    public Guid ProductId { get; init; }
    public Guid ProductImageId { get; init; }
}

public class DeleteProductImageCommandHandler(
    IProductRepository productRepository,
    IImageService imageService,
    IMapper mapper) : IRequestHandler<DeleteProductImageCommand, ServiceResponse>
{
    public async Task<ServiceResponse> Handle(DeleteProductImageCommand request,
        CancellationToken cancellationToken)
    {
        var existingProduct = await productRepository.GetById(request.ProductId, cancellationToken);

        return await existingProduct.Match(
            async product => await HandleImageDeletion(product, request.ProductImageId, cancellationToken),
            () => Task.FromResult(
                ServiceResponse.NotFoundResponse("Product not found")));
    }

    private async Task<ServiceResponse> HandleImageDeletion(Product product,
        Guid productImageId, CancellationToken cancellationToken)
    {
        var productImage = product.Images.FirstOrDefault(x => x.Id == productImageId);
        if (productImage is null)
        {
            return ServiceResponse.NotFoundResponse("Product image not found");
        }

        var deleteResult = await imageService.DeleteImageAsync(ImagePaths.ProductImagesPath, productImage.FilePath!);

        return await deleteResult.Match<Task<ServiceResponse>>(
            async _ =>
            {
                product.Images.Remove(productImage);
                await productRepository.Update(mapper.Map<UpdateProductModel>(product), cancellationToken);
                return ServiceResponse.OkResponse("Product image deleted");
            },
            error => Task.FromResult(ServiceResponse.InternalServerErrorResponse(error)));
    }
}