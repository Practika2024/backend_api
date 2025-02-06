using Application.Common;
using Application.Common.Interfaces.Repositories;
using Application.Exceptions;
using Application.Services.HashPasswordService;
using Application.Services.TokenService;
using Application.ViewModels;
using Domain.Authentications;
using Domain.Authentications.Users;

namespace Application.Services.AuthenticationServices.SignUpService;

public class SignUpService : ISignUpService
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IHashPasswordService _hashPasswordService;

    public SignUpService(IUserRepository userRepository, IJwtTokenService jwtTokenService,
        IHashPasswordService hashPasswordService)
    {
        _userRepository = userRepository;
        _jwtTokenService = jwtTokenService;
        _hashPasswordService = hashPasswordService;
    }

    public async Task<Result<JwtVM, AuthenticationException>> SignUpAsync(string email, string password, string name,
        string surname,
        string patronymic, CancellationToken cancellationToken)
    {
        var existingUser = await _userRepository.SearchByEmail(email, cancellationToken);

        return await existingUser.Match<Task<Result<JwtVM, AuthenticationException>>>(
            u => Task.FromResult<Result<JwtVM, AuthenticationException>>(
                new UserByThisEmailAlreadyExistsAuthenticationException(u.Id)),
            async () => await CreateUser(email, password, name, surname, patronymic, cancellationToken)
        );
    }

    private async Task<Result<JwtVM, AuthenticationException>> CreateUser(string email, string password, string name,
        string surname,
        string patronymic,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = UserId.New();
            var hashedPassword = _hashPasswordService.HashPassword(password);
            var user = User.New(userId, email, name, surname, patronymic, hashedPassword);

            await _userRepository.Create(user, cancellationToken);

            var token = await _jwtTokenService
                .GenerateTokensAsync(await _userRepository
                    .AddRole(user.Id, AuthSettings.OperatorRole, cancellationToken), cancellationToken);

            return token;
        }
        catch (Exception exception)
        {
            return new AuthenticationUnknownException(UserId.Empty, exception);
        }
    }
}