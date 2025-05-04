using Application.Common.Interfaces;
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

public record UpdateProductImagesCommand : IRequest<ServiceResponse>
{
    public Guid ProductId { get; init; }
    public required IFormFileCollection NewImages { get; init; }
    public List<Guid> ImagesToDelete { get; init; } = [];
}

public class UpdateProductImagesCommandHandler(
    IProductRepository productRepository,
    IImageService imageService,
    IMapper mapper,
    IHttpContextAccessor httpContextAccessor,
    IUserProvider userProvider)
    : IRequestHandler<UpdateProductImagesCommand, ServiceResponse>
{
    public async Task<ServiceResponse> Handle(UpdateProductImagesCommand request, CancellationToken cancellationToken)
    {
        var product = await productRepository.GetById(request.ProductId, cancellationToken);
        return await product.Match(
            async p =>
            {
                // Видалення
                foreach (var id in request.ImagesToDelete)
                {
                    var image = p.Images.FirstOrDefault(x => x.Id == id);
                    if (image != null)
                    {
                        imageService.DeleteImageAsync(ImagePaths.ProductImagesPath, image.FileName!);
                        p.Images.Remove(image);
                    }
                }

                // Додавання
                var savedImages =
                    await imageService.SaveImagesFromFilesAsync(ImagePaths.ProductImagesPath, request.NewImages);

                var baseUrl =
                    $"{httpContextAccessor.HttpContext!.Request.Scheme}://{httpContextAccessor.HttpContext.Request.Host}/";

                foreach (var fileName in savedImages)
                {
                    p.Images.Add(new ProductImage
                    {
                        FileName = fileName!,
                        FilePath = $"{baseUrl}{ImagePaths.ProductImagesPathForUrl}/{fileName}"
                    });
                }

                try
                {
                    p.ModifiedBy = userProvider.GetUserId();
                    var updated = await productRepository.Update(mapper.Map<UpdateProductModel>(p), cancellationToken);
                    return ServiceResponse.OkResponse("Images updated", updated);
                }
                catch (Exception e)
                {
                    return ServiceResponse.InternalServerErrorResponse(e.Message, e);
                }
            },
            () => Task.FromResult(ServiceResponse.NotFoundResponse("Product not found"))
        );
    }
}