using Application.Authentications.Exceptions;
using Application.Common;
using Application.Common.Interfaces.Repositories;
using Application.Models.UserModels;
using Application.Services.HashPasswordService;
using Application.Services.TokenService;
using Domain.Authentications.Users;
using MediatR;

namespace Application.Commands.Authentications.Commands;

public class SignInCommand: IRequest<Result<JwtModel, AuthenticationException>>
{
    public required string Email { get; init; }
    public required string Password { get; init; }
}

public class SignInCommandHandler(IUserRepository userRepository, IJwtTokenService jwtTokenService, IHashPasswordService hashPasswordService) 
    : IRequestHandler<SignInCommand, Result<JwtModel, AuthenticationException>>
{
    public async Task<Result<JwtModel, AuthenticationException>> Handle(
        SignInCommand request,
        CancellationToken cancellationToken)
    {
        var existingUser = await userRepository.SearchByEmail(request.Email, cancellationToken);
        
        return await existingUser.Match(
            async u => await SignIn(u, request.Password, cancellationToken),
            () => Task.FromResult<Result<JwtModel, AuthenticationException>>(new EmailOrPasswordAreIncorrectException()));
    }
    private async Task<Result<JwtModel, AuthenticationException>> SignIn(
        UserEntity user,
         string password,
         CancellationToken cancellationToken)
     {
         string storedHash = user.PasswordHash;

         if (!hashPasswordService.VerifyPassword(password, storedHash))
         {
             return new EmailOrPasswordAreIncorrectException();
         }

         try
         {
             var token = await jwtTokenService.GenerateTokensAsync(user, cancellationToken);
             return token;
         }
         catch (Exception exception)
         {
             return new AuthenticationUnknownException(user.Id, exception);
         }
     }
}