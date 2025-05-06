using Application.Common.Interfaces;
using Application.Common.Interfaces.Queries;
using Application.Common.Interfaces.Repositories;
using Application.Services;
using Application.Services.ImageService;
using Application.Settings;
using AutoMapper;
using Domain.Containers;
using Domain.Containers.Models;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Commands.Containers.Commands;

public class UpdateContainerImageCommand : IRequest<ServiceResponse>
{
    public Guid ContainerId { get; set; }
    public required IFormFile File { get; set; }
}

public class UpdateContainerImageCommandHandler(
    IContainerRepository containerRepository,
    IContainerQueries containerQueries,
    IImageService imageService,
    IHttpContextAccessor httpContextAccessor,
    IMapper mapper,
    IUserProvider userProvider) : IRequestHandler<UpdateContainerImageCommand, ServiceResponse>
{
    public async Task<ServiceResponse> Handle(UpdateContainerImageCommand request, CancellationToken cancellationToken)
    {
        var containerId = request.ContainerId;
        var existingContainer = await containerQueries.GetById(containerId, cancellationToken);

        return await existingContainer.Match(
            async container => await UpdateImage(container, request.File, cancellationToken),
            () => Task.FromResult<ServiceResponse>(
                ServiceResponse.NotFoundResponse("Container not found")));
    }

    private async Task<ServiceResponse> UpdateImage(Container container, IFormFile image,
        CancellationToken cancellationToken)
    {
        var imageName = container.FilePath?.Split('/').LastOrDefault();

        var newImageName =
            await imageService.SaveImageFromFileAsync(ImagePaths.ContainerImagesPath, image, imageName);

        if (newImageName == null)
        {
            return ServiceResponse.BadRequestResponse("No image uploaded");
        }

        var baseUrl =
            $"{httpContextAccessor.HttpContext!.Request.Scheme}://{httpContextAccessor.HttpContext.Request.Host}/";

        container.FilePath = $"{baseUrl}{ImagePaths.ContainerImagesPathForUrl}/{newImageName}";
        container.ModifiedBy = userProvider.GetUserId();

        try
        {
            var updatedContainer = await containerRepository.Update(mapper.Map<UpdateContainerModel>(container), cancellationToken);

            return ServiceResponse.OkResponse("Container image updated successfully", updatedContainer);
        }
        catch (Exception e)
        {
            return ServiceResponse.InternalServerErrorResponse(e.Message);
        }
    }
}