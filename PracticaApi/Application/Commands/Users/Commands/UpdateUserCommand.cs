using Application.Commands.Users.Exceptions;
using Application.Common;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Repositories;
using Application.Services.TokenService;
using Domain.Users;
using Domain.Users.Models;
using MediatR;

namespace Application.Commands.Users.Commands;

public record UpdateUserCommand : IRequest<Result<User, UserException>>
{
    public required Guid UserId { get; init; }
    public required string Email { get; init; }
    public required string? Name { get; init; }
    public required string? Surname { get; init; }
    public required string? Patronymic { get; init; }
}

public class UpdateUserCommandHandle(IUserRepository userRepository, IUserProvider userProvider)
    : IRequestHandler<UpdateUserCommand, Result<User, UserException>>
{
    public async Task<Result<User, UserException>> Handle(UpdateUserCommand request,
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
                    e => Task.FromResult<Result<User, UserException>>
                        (new UserByThisEmailAlreadyExistsException(userId)),
                    async () => await UpdateEntity(u, request.Email, request.Name, request.Surname, request.Patronymic,
                        cancellationToken));
            },
            () => Task.FromResult<Result<User, UserException>>
                (new UserNotFoundException(userId)));
    }

    private async Task<Result<User, UserException>> UpdateEntity(
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
            return updatedUser;
        }
        catch (Exception exception)
        {
            return new UserUnknownException(user.Id, exception);
        }
    }
}