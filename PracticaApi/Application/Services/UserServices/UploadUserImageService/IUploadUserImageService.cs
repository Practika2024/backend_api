using Application.Common;
using Application.Exceptions;
using Application.ViewModels;
using Microsoft.AspNetCore.Http;

namespace Application.Services.UserServices.UploadUserImageService;


public interface IUploadUserImageService
{
    Task<Result<JwtVM, UserException>> UploadUserImageAsync(Guid userId, IFormFile imageFile, CancellationToken cancellationToken);
}