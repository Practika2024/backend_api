using System.Net;
using Application.Commands.Users.Exceptions;
using Application.Common;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Repositories;
using Application.Services;
using Application.Services.TokenService;
using Domain.Users;
using Domain.Users.Models;
using MediatR;

namespace Application.Commands.Users.Commands;

public record UpdateUserCommand : IRequest<ServiceResponse>
{
    public required Guid UserId { get; init; }
    public required string Email { get; init; }
    public required string? Name { get; init; }
    public required string? Surname { get; init; }
    public required string? Patronymic { get; init; }
}

public class UpdateUserCommandHandle(IUserRepository userRepository, IUserProvider userProvider)
    : IRequestHandler<UpdateUserCommand, ServiceResponse>
{
    public async Task<ServiceResponse> Handle(UpdateUserCommand request,
        CancellationToken cancellationToken)
    {
        var userId = request.UserId;
        var existingUser = await userRepository.GetById(userId, cancellationToken);

        return await existingUser.Match(
            async u =>
            {
                var existingEmail =
                    await userRepository.SearchByEmailForUpdate(userId, request.Email, cancellationToken);

                return await existingEmail.Match(
                    e => Task.FromResult<ServiceResponse>
                        (ServiceResponse.GetResponse("User with this email already exists", 
                            false, null, HttpStatusCode.Conflict)),
                    async () => await UpdateEntity(u, request.Email, request.Name, request.Surname, request.Patronymic,
                        cancellationToken));
            },
            () => Task.FromResult<ServiceResponse>
                (ServiceResponse.NotFoundResponse("User not found")));
    }

    private async Task<ServiceResponse> UpdateEntity(
        User user,
        string email,
        string name,
        string surname,
        string patronymic,
        CancellationToken cancellationToken)
    {
        try
        {
            var userModel = new UpdateUserModel
            {
                Id = user.Id,
                Email = email,
                Name = name,
                Surname = surname,
                Patronymic = patronymic,
                ModifiedBy = userProvider.GetUserId()
            };

            var updatedUser = await userRepository.Update(userModel, cancellationToken);
            return ServiceResponse.OkResponse("Updated user", updatedUser);
        }
        catch (Exception exception)
        {
            return ServiceResponse.InternalServerErrorResponse(exception.Message, exception);
        }
    }
}