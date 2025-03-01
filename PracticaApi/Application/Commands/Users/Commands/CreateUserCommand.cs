using Application.Commands.Authentications.Exceptions;
using Application.Common;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Repositories;
using Application.Services.HashPasswordService;
using Application.Settings;
using Domain.Users;
using Domain.Users.Models;
using MediatR;

namespace Application.Commands.Users.Commands;

public class CreateUserCommand : IRequest<Result<User, AuthenticationException>>
{
    public required string Email { get; init; }
    public required string? Surname { get; init; }
    public required string? Patronymic { get; init; }
    public required string Password { get; init; }
    public required string? Name { get; init; }
}

public class CreateUserCommandHandler(
    IUserRepository userRepository,
    IHashPasswordService hashPasswordService,
    IUserProvider userProvider)
    : IRequestHandler<CreateUserCommand, Result<User, AuthenticationException>>
{
    public async Task<Result<User, AuthenticationException>> Handle(
        CreateUserCommand request,
        CancellationToken cancellationToken)
    {
        var existingUser = await userRepository.SearchByEmail(request.Email, cancellationToken);

         return await existingUser.Match<Task<Result<User, AuthenticationException>>>(
            u => Task.FromResult<Result<User, AuthenticationException>>(
                new UserByThisEmailAlreadyExistsAuthenticationException(u.Id)),
            async () => await SignUp(request, cancellationToken)
        );
    }

    private async Task<Result<User, AuthenticationException>> SignUp(
        CreateUserCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var userId = Guid.NewGuid();
            var userModel = new CreateUserModel
            {
                Id = userId,
                Email = request.Email,
                Name = request.Name,
                Surname = request.Surname,
                Patronymic = request.Patronymic,
                PasswordHash = hashPasswordService.HashPassword(request.Password),
                RoleId = AuthSettings.OperatorRole,
                CreatedBy = userProvider.GetUserId()
            };
            User userEntity = await userRepository.Create(userModel, cancellationToken);

            return userEntity;
        }
        catch (Exception exception)
        {
            return new AuthenticationUnknownException(Guid.Empty, exception);
        }
    }
}