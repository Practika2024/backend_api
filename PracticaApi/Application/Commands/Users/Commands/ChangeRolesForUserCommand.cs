// using Application.Common;
// using Application.Common.Interfaces.Repositories;
// using Application.Exceptions;
// using Domain.UserModels;
// using MediatR;
//
// namespace Application.Commands.Users.Commands;
// public record ChangeRolesForUserCommand : IRequest<Result<User, UserException>>
// {
//     public required Guid UserId { get; init; }
//     public List<string> Roles { get; init; } = new();
// }
//
// public class ChangeRolesForUserCommandHandler(
//     IUserRepository userRepository) : IRequestHandler<ChangeRolesForUserCommand, Result<User, UserException>>
// {
//     public async Task<Result<User, UserException>> Handle(
//         ChangeRolesForUserCommand request,
//         CancellationToken cancellationToken)
//     {
//         var userId = request.UserId;
//         var existingUser = await userRepository.GetById(userId, cancellationToken);
//
//         return await existingUser.Match(
//             async user =>
//             {
//                 try
//                 {
//                     var updateRolesModel = new UpdateRolesModel
//                     {
//                         UserId = userId,
//                         RoleIds = request.Roles
//                     };
//                     var updatedUser = await userRepository.UpdateRoles(updateRolesModel, cancellationToken);
//                     return updatedUser;
//                 }
//                 catch (Exception exception)
//                 {
//                     return new UserUnknownException(user.Id, exception);
//                 }
//             },
//             () => Task.FromResult<Result<User, UserException>>(new UserNotFoundException(userId))
//         );
//     }
// }