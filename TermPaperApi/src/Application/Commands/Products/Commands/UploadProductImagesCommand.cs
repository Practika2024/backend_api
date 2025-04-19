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

namespace Application.Commands.Products.Commands;

public record UploadProductImagesCommand : IRequest<Result<Product, ProductException>>
{
    public Guid ProductId { get; init; }
    public IFormFileCollection ImagesFiles { get; init; }
}

public class UploadProductImagesCommandHandler(
    IProductRepository productRepository,
    IImageService imageService,
    IMapper mapper) : IRequestHandler<UploadProductImagesCommand, Result<Product, ProductException>>
{
    public async Task<Result<Product, ProductException>> Handle(UploadProductImagesCommand request,
        CancellationToken cancellationToken)
    {
        var productId = request.ProductId;
        var existingProduct = await productRepository.GetById(productId, cancellationToken);

        return await existingProduct.Match(
            async product => await UploadImages(product, request.ImagesFiles, cancellationToken),
            () => Task.FromResult<Result<Product, ProductException>>(
                new ProductNotFoundException(productId)));
    }
    
    private async Task<Result<Product, ProductException>> UploadImages(
        Product product,
        IFormFileCollection imagesFiles,
        CancellationToken cancellationToken)
    {
        var imageSaveResult = await imageService.SaveImagesFromFilesAsync(ImagePaths.ProductImagesPath, imagesFiles);

        if (imageSaveResult == null)
        {
            return new ImageSaveException(product.Id);
        }
        
        foreach (var imageName in imageSaveResult)
        {
            product.Images.Add(new ProductImage() { FilePath = imageName });
        }
                
        var productWithImages = await productRepository.Update(mapper.Map<UpdateProductModel>(product), cancellationToken);
        return productWithImages;
    }
}