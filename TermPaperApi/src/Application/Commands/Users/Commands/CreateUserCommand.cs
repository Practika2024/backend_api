using System.Net;
using Application.Commands.Authentications.Exceptions;
using Application.Common;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Repositories;
using Application.Services;
using Application.Services.HashPasswordService;
using Application.Settings;
using Domain.Users;
using Domain.Users.Models;
using MediatR;

namespace Application.Commands.Users.Commands;

public class CreateUserCommand : IRequest<ServiceResponse>
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
    : IRequestHandler<CreateUserCommand, ServiceResponse>
{
    public async Task<ServiceResponse> Handle(
        CreateUserCommand request,
        CancellationToken cancellationToken)
    {
        var existingUser = await userRepository.SearchByEmail(request.Email, cancellationToken);

         return await existingUser.Match<Task<ServiceResponse>>(
            u => Task.FromResult<ServiceResponse>(
                ServiceResponse
                    .GetResponse("User with this email already exists", 
                        false, null, HttpStatusCode.Conflict)
                ),
            async () => await SignUp(request, cancellationToken)
        );
    }

    private async Task<ServiceResponse> SignUp(
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

            return ServiceResponse.OkResponse("User created", userEntity);
        }
        catch (Exception exception)
        {
            return ServiceResponse.InternalServerErrorResponse(exception.Message);
        }
    }
}