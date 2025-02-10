using Application.Common;
using Application.Common.Interfaces.Repositories;
using Application.Exceptions;
using Application.Services.TokenService;
using Application.ViewModels;
using Domain.Authentications.Users;

namespace Application.Services.UserServices.UpdateUserService;

public class UpdateUserService : IUpdateUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenService _jwtTokenService;

    public UpdateUserService(IUserRepository userRepository, IJwtTokenService jwtTokenService)
    {
        _userRepository = userRepository;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<Result<JwtVM, UserException>> UpdateUserAsync(Guid userId, string email, string userName, CancellationToken cancellationToken)
    {
        var userIdObj = new UserId(userId);
        var existingUser = await _userRepository.GetById(userIdObj, cancellationToken);

        return await existingUser.Match<Task<Result<JwtVM, UserException>>>(
            async user =>
            {
                var existingEmail = await _userRepository.SearchByEmailForUpdate(userIdObj, email, cancellationToken);

                return await existingEmail.Match<Task<Result<JwtVM, UserException>>>(
                    _ => Task.FromResult<Result<JwtVM, UserException>>(new UserByThisEmailAlreadyExistsException(userIdObj)),
                    async () => await UpdateEntity(user, email, userName, cancellationToken)
                );
            },
            () => Task.FromResult<Result<JwtVM, UserException>>(new UserNotFoundException(userIdObj))
        );
    }

    private async Task<Result<JwtVM, UserException>> UpdateEntity(UserEntity userEntity, string email, string userName, CancellationToken cancellationToken)
    {
        try
        {
            userEntity.UpdateUser(email, userName);
            var updatedUser = await _userRepository.Update(userEntity, cancellationToken);
            return await _jwtTokenService.GenerateTokensAsync(updatedUser, cancellationToken);
        }
        catch (Exception exception)
        {
            return new UserUnknownException(userEntity.Id, exception);
        }
    }
}