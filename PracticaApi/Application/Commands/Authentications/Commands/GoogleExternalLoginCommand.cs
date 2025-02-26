using Application.Commands.Authentications.Exceptions;
using Application.Common;
using Application.Common.Interfaces.Repositories;
using Application.Services.HashPasswordService;
using Application.Services.TokenService;
using Application.Settings;
using Domain.Users.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace Application.Commands.Authentications.Commands;

public record GoogleExternalLoginCommand : IRequest<Result<JwtModel, AuthenticationException>>
{
    public required ExternalLoginModel Model { get; init; }
}
public class GoogleExternalLoginCommandHandler : IRequestHandler<GoogleExternalLoginCommand, Result<JwtModel, AuthenticationException>>
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IConfiguration _configuration;
    private readonly IHashPasswordService _hashPasswordService;

    public GoogleExternalLoginCommandHandler(
        IUserRepository userRepository,
        IJwtTokenService jwtTokenService,
        IConfiguration configuration,
        IHashPasswordService hashPasswordService)
    {
        _userRepository = userRepository;
        _jwtTokenService = jwtTokenService;
        _configuration = configuration;
        _hashPasswordService = hashPasswordService;
    }

    public async Task<Result<JwtModel, AuthenticationException>> Handle(GoogleExternalLoginCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (request.Model == null || string.IsNullOrEmpty(request.Model.Token))
            {
                return new MissingGoogleTokenException();
            }

            var clientId = _configuration["GoogleAuthSettings:ClientId"];
            var payload = await _jwtTokenService.VerifyGoogleToken(request.Model);

            if (payload == null)
            {
                return new InvalidGoogleTokenException();
            }

            var info = new UserLoginInfo(request.Model.Provider, payload.Subject, request.Model.Provider);
            var user = await _userRepository.FindByLoginAsync(info.LoginProvider, info.ProviderKey, cancellationToken);

            if (user == null)
            {
                user = await _userRepository.FindByEmailAsync(payload.Email, cancellationToken);

                if (user == null)
                {
                    var userId = Guid.NewGuid();

                    var randomPassword = GenerateRandomPassword();

                    var userModel = new CreateUserModel
                    {
                        Id = userId,
                        Email = payload.Email,
                        Name = payload.GivenName,
                        Surname = payload.FamilyName,
                        RoleId = AuthSettings.OperatorRole,
                        PasswordHash = _hashPasswordService.HashPassword(randomPassword)
                    };

                    user = await _userRepository.Create(userModel, cancellationToken);
                }

                var loginResult = await _userRepository.AddLoginAsync(user, info, cancellationToken);
                if (!loginResult.Succeeded)
                {
                    return new FailedToAddGoogleLoginException();
                }
            }

            var tokens = await _jwtTokenService.GenerateTokensAsync(user, cancellationToken);
            return tokens;
        }
        catch (Exception ex)
        {
            return new AuthenticationUnknownException(Guid.Empty, ex);
        }
    }

    private string GenerateRandomPassword()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()";
        var random = new Random();
        return new string(Enumerable.Repeat(chars, 6)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }
}