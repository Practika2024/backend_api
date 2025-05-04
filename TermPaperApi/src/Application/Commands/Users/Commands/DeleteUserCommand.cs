using Application.Commands.Users.Exceptions;
using Application.Common;
using Application.Common.Interfaces.Repositories;
using Application.Services;
using Domain.Users;
using Domain.Users.Models;
using MediatR;
using Microsoft.AspNetCore.Hosting;

namespace Application.Commands.Users.Commands;

public record DeleteUserCommand : IRequest<ServiceResponse>
{
    public required Guid UserId { get; init; }
}

public class DeleteUserCommandHandler(
    IUserRepository userRepository)
    : IRequestHandler<DeleteUserCommand, ServiceResponse>
{
    public async Task<ServiceResponse> Handle(
        DeleteUserCommand request,
        CancellationToken cancellationToken)
    {
        var userId = request.UserId;

        var existingUser = await userRepository.GetById(userId, cancellationToken);

        return await existingUser.Match<Task<ServiceResponse>>(
            async user =>
            {
                try
                {
                    var deleteModel = new DeleteUserModel { Id = userId };
                    var deletedUser = await userRepository.Delete(deleteModel, cancellationToken);
                    return ServiceResponse.OkResponse("User deleted", deletedUser);
                }
                catch (Exception exception)
                {
                    return ServiceResponse.InternalServerErrorResponse(exception.Message, exception);
                }
            },
            () => Task.FromResult<ServiceResponse>(
                ServiceResponse.NotFoundResponse("User not found"))
        );
    }
}