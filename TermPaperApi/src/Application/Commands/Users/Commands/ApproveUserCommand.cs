using Application.Commands.Users.Exceptions;
using Application.Common;
using Application.Common.Interfaces.Repositories;
using Application.Services;
using Domain.Users;
using Domain.Users.Models;
using MediatR;
using Microsoft.AspNetCore.Hosting;

namespace Application.Commands.Users.Commands;

public record ApproveUserCommand : IRequest<ServiceResponse>
{
    public required Guid UserId { get; init; }
}

public class ApproveUserCommandHandler(
    IUserRepository userRepository)
    : IRequestHandler<ApproveUserCommand, ServiceResponse>
{
    public async Task<ServiceResponse> Handle(
        ApproveUserCommand request,
        CancellationToken cancellationToken)
    {
        var userId = request.UserId;

        var existingUser = await userRepository.GetById(userId, cancellationToken);

        return await existingUser.Match(
            async user =>
            {
                try
                {
                    var approvedUser = await userRepository.ApproveUser(user.Id, cancellationToken);
                    return ServiceResponse.OkResponse("User approved", approvedUser);
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