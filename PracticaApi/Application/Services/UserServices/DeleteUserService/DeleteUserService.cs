using Application.Common;
using Application.Common.Interfaces.Repositories;
using Application.Exceptions;
using Domain.Authentications.Users;
using Microsoft.AspNetCore.Hosting;

namespace Application.Services.UserServices.DeleteUserService;

public class DeleteUserService : IDeleteUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public DeleteUserService(IUserRepository userRepository, IWebHostEnvironment webHostEnvironment)
    {
        _userRepository = userRepository;
        _webHostEnvironment = webHostEnvironment;
    }

    public async Task<Result<User, UserException>> DeleteUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        var userIdObj = new UserId(userId);
        var existingUser = await _userRepository.GetById(userIdObj, cancellationToken);

        return await existingUser.Match<Task<Result<User, UserException>>>(
            async user =>
            {
                DeleteImageByUser(user);
                return await _userRepository.Delete(user, cancellationToken);
            },
            () => Task.FromResult<Result<User, UserException>>(new UserNotFoundException(userIdObj))
        );
    }

    private void DeleteImageByUser(User user)
    {
        var userImage = user.UserImage?.FilePath;
        if (!string.IsNullOrEmpty(userImage))
        {
            var fullPath = Path.Combine(_webHostEnvironment.ContentRootPath, ImagePaths.UserImagePath, userImage);
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
        }
    }
}