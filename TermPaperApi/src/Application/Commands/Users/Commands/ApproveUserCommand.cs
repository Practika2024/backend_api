using Application.Common.Interfaces.Repositories;
using Application.Services;
using MediatR;

namespace Application.Commands.Users.Commands;

public record ApproveUserCommand : IRequest<ServiceResponse>
{
    public required Guid UserId { get; init; }
    public required bool IsUserApproved { get; init; }
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
                    var approvedUser = await userRepository.ApproveUser(user.Id, request.IsUserApproved, cancellationToken);
                    return ServiceResponse.OkResponse("User approval is configured", approvedUser);
                }
                catch (Exception exception)
                {
                    return ServiceResponse.InternalServerErrorResponse(exception.Message, exception);
                }
            },
            () => Task.FromResult(
                ServiceResponse.NotFoundResponse("User not found"))
        );
    }
}