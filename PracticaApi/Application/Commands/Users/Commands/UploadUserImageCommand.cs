using Application.Commands.Users.Exceptions;
using Application.Common;
using Application.Common.Interfaces.Repositories;
using Application.Models.UserModels;
using Application.Services.ImageService;
using Application.Services.TokenService;
using Application.ViewModels;
using Domain.Authentications.Users;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Commands.Users.Commands;

public record UploadUserImageCommand : IRequest<Result<JwtModel, UserException>>
{
    public Guid UserId { get; init; }
    public IFormFile ImageFile { get; init; }
}

public class UploadUserImageCommandHandler(
    IUserRepository userRepository,
    IJwtTokenService jwtTokenService,
    IImageService imageService)
    : IRequestHandler<UploadUserImageCommand, Result<JwtModel, UserException>>
{
    public async Task<Result<JwtModel, UserException>> Handle(UploadUserImageCommand request,
        CancellationToken cancellationToken)
    {
        var userId = new UserId(request.UserId);
        var existingUser = await userRepository.GetById(userId, cancellationToken);

        return await existingUser.Match<Task<Result<JwtModel, UserException>>>(
            async user => await UploadOrReplaceImage(user, request.ImageFile, cancellationToken),
            () => Task.FromResult<Result<JwtModel, UserException>>(
                new UserNotFoundException(userId)));
    }

    private async Task<Result<JwtModel, UserException>> UploadOrReplaceImage(
        UserEntity user,
        IFormFile imageFile,
        CancellationToken cancellationToken)
    {
        var imageSaveResult = await imageService.SaveImageFromFileAsync(
            ImagePaths.UserImagePath,
            imageFile,
            user.UserImage?.FilePath
        );

        return await imageSaveResult.Match<Task<Result<JwtModel, UserException>>>(
            async imageName =>
            {
                var updateUserImageModel = new UpdateUserImageModel
                {
                    UserId = user.Id,
                    FilePath = imageName
                };
                var userWithNewImage = await userRepository.UpdateUserImage(updateUserImageModel, cancellationToken);
                return await jwtTokenService.GenerateTokensAsync(userWithNewImage, cancellationToken);
            },
            () => Task.FromResult<Result<JwtModel, UserException>>(new ImageSaveException(user.Id))
        );
    }
}