using Application.Common;
using Application.Common.Interfaces.Repositories;
using Application.Exceptions;
using Application.Services.HashPasswordService;
using Application.Services.TokenService;
using Application.ViewModels;
using Domain.Authentications.Users;

namespace Application.Services.AuthenticationServices.SignInService;

public class SignInService : ISignInService
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IHashPasswordService _hashPasswordService;

    public SignInService(IUserRepository userRepository, IJwtTokenService jwtTokenService, IHashPasswordService hashPasswordService)
    {
        _userRepository = userRepository;
        _jwtTokenService = jwtTokenService;
        _hashPasswordService = hashPasswordService;
    }

    public async Task<Result<JwtVM, AuthenticationException>> SignInAsync(string email, string password, CancellationToken cancellationToken)
    {
        var existingUser = await _userRepository.SearchByEmail(email, cancellationToken);

        return await existingUser.Match<Task<Result<JwtVM, AuthenticationException>>>(
            async user => await VerifyPassword(user, password, cancellationToken),
            () => Task.FromResult<Result<JwtVM, AuthenticationException>>(new EmailOrPasswordAreIncorrectException())
        );
    }

    private async Task<Result<JwtVM, AuthenticationException>> VerifyPassword(UserEntity userEntity, string password, CancellationToken cancellationToken)
    {
        if (!_hashPasswordService.VerifyPassword(password, userEntity.PasswordHash))
        {
            return new EmailOrPasswordAreIncorrectException();
        }

        try
        {
            var token = await _jwtTokenService.GenerateTokensAsync(userEntity, cancellationToken);
            return token;
        }
        catch (Exception exception)
        {
            return new AuthenticationUnknownException(userEntity.Id, exception);
        }
    }
}