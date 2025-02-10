using Application.Common;
using Application.Common.Interfaces.Repositories;
using Application.Exceptions;
using Application.Services.ImageService;
using Application.Services.TokenService;
using Application.ViewModels;
using Domain.Authentications.Users;
using Microsoft.AspNetCore.Http;

namespace Application.Services.UserServices.UploadUserImageService;

public class UploadUserImageService : IUploadUserImageService
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IImageService _imageService;

    public UploadUserImageService(IUserRepository userRepository, IJwtTokenService jwtTokenService, IImageService imageService)
    {
        _userRepository = userRepository;
        _jwtTokenService = jwtTokenService;
        _imageService = imageService;
    }

    public async Task<Result<JwtVM, UserException>> UploadUserImageAsync(Guid userId, IFormFile imageFile, CancellationToken cancellationToken)
    {
        var userIdObj = new UserId(userId);
        var existingUser = await _userRepository.GetById(userIdObj, cancellationToken);

        return await existingUser.Match<Task<Result<JwtVM, UserException>>>(
            async user => await UploadOrReplaceImage(user, imageFile, cancellationToken),
            () => Task.FromResult<Result<JwtVM, UserException>>(new UserNotFoundException(userIdObj))
        );
    }

    private async Task<Result<JwtVM, UserException>> UploadOrReplaceImage(UserEntity userEntity, IFormFile imageFile, CancellationToken cancellationToken)
    {
        var imageSaveResult = await _imageService.SaveImageFromFileAsync(ImagePaths.UserImagePath, imageFile, userEntity.UserImage?.FilePath);

        return await imageSaveResult.Match<Task<Result<JwtVM, UserException>>>(
            async imageName =>
            {
                var imageEntity = UserImageEntity.New(UserImageId.New(), userEntity.Id, imageName);
                userEntity.UpdateUserImage(imageEntity);
                var userWithNewImage = await _userRepository.Update(userEntity, cancellationToken);
                return await _jwtTokenService.GenerateTokensAsync(userWithNewImage, cancellationToken);
            },
            () => Task.FromResult<Result<JwtVM, UserException>>(new ImageSaveException(userEntity.Id))
        );
    }
}