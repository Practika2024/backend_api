using System.Net;
using Application.Commands.Products.Exceptions;
using Application.Common;
using Application.Common.Interfaces.Repositories;
using Application.Services;
using Application.Services.ImageService;
using Application.Settings;
using AutoMapper;
using Domain.Products;
using Domain.Products.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Application.Commands.Products.Commands;

public record UploadProductImagesCommand : IRequest<ServiceResponse>
{
    public Guid ProductId { get; init; }
    public IFormFileCollection ImagesFiles { get; init; }
}

public class UploadProductImagesCommandHandler(
    IProductRepository productRepository,
    IImageService imageService,
    IMapper mapper,
    IHttpContextAccessor httpContextAccessor) : IRequestHandler<UploadProductImagesCommand, ServiceResponse>
{
    public async Task<ServiceResponse> Handle(UploadProductImagesCommand request,
        CancellationToken cancellationToken)
    {
        var productId = request.ProductId;
        var existingProduct = await productRepository.GetById(productId, cancellationToken);

        return await existingProduct.Match(
            async product => await UploadImages(product, request.ImagesFiles, cancellationToken),
            () => Task.FromResult<ServiceResponse>(
                ServiceResponse.NotFoundResponse("Product not found")));
    }
    
    private async Task<ServiceResponse> UploadImages(
        Product product,
        IFormFileCollection imagesFiles,
        CancellationToken cancellationToken)
    {
        var imageSaveResult = await imageService.SaveImagesFromFilesAsync(ImagePaths.ProductImagesPath, imagesFiles);

        if (imageSaveResult.Count == 0)
        {
            return ServiceResponse.BadRequestResponse("No images uploaded");
        }

        var httpContext = httpContextAccessor.HttpContext;
        if (httpContext == null)
        {
            return ServiceResponse.InternalServerErrorResponse("HTTP context is unavailable");
        }

        var scheme = httpContext.Request.Scheme;
        var host = httpContext.Request.Host;

        var baseUrl = $"{scheme}://{host}/";

        foreach (var imageName in imageSaveResult)
        {
            var relativePath = $"{ImagePaths.ProductImagesPathForUrl}/{imageName}";
            var fullPath = $"{baseUrl}{relativePath}";

            product.Images.Add(new ProductImage()
            {
                FileName = imageName!,
                FilePath = fullPath
            });
        }

        var productWithImages = await productRepository.Update(mapper.Map<UpdateProductModel>(product), cancellationToken);
        return ServiceResponse.OkResponse("Images uploaded", productWithImages);
    }
}