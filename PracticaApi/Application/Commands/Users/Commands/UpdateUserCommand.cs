// using Application.Commands.Users.Exceptions;
// using Application.Common;
// using Application.Common.Interfaces.Repositories;
// using Application.Services.TokenService;
// using Domain.Users;
// using MediatR;
//
// namespace Application.Commands.Users.Commands;
//
// public record UpdateUserCommand : IRequest<Result<JwtModel, UserException>>
// {
//     public required Guid UserId { get; init; }
//     public required string Email { get; init; }
//     public required string? Name { get; init; }
//     public required string? Surname { get; init; }
//     public required string? Patronymic { get; init; }
// }
//
// public class UpdateUserCommandHandle(IUserRepository userRepository, IJwtTokenService jwtTokenService)
//     : IRequestHandler<UpdateUserCommand, Result<JwtModel, UserException>>
// {
//     public async Task<Result<JwtModel, UserException>> Handle(UpdateUserCommand request,
//         CancellationToken cancellationToken)
//     {
//         var userId = request.UserId;
//         var existingUser = await userRepository.GetById(userId, cancellationToken);
//
//         return await existingUser.Match(
//             async u =>
//             {
//                 var existingEmail =
//                     await userRepository.SearchByEmailForUpdate(userId, request.Email, cancellationToken);
//
//                 return await existingEmail.Match(
//                     e => Task.FromResult<Result<JwtModel, UserException>>
//                         (new UserByThisEmailAlreadyExistsException(userId)),
//                     async () => await UpdateEntity(u, request.Email, request.Name, request.Surname, request.Patronymic,
//                         cancellationToken));
//             },
//             () => Task.FromResult<Result<JwtModel, UserException>>
//                 (new UserNotFoundException(userId)));
//     }
//
//     private async Task<Result<JwtModel, UserException>> UpdateEntity(
//         User user,
//         string email,
//         string name,
//         string surname,
//         string patronymic,
//         CancellationToken cancellationToken)
//     {
//         try
//         {
//             user.UpdateUser(email, name, surname, patronymic);
//             var userModel = new UpdateUserModel
//             {
//                 Id = user.Id,
//                 Email = user.Email,
//                 Name = user.Name,
//                 Surname = user.Surname,
//                 Patronymic = user.Patronymic,
//             };
//
//             var updatedUser = await userRepository.Update(userModel, cancellationToken);
//             return await jwtTokenService.GenerateTokensAsync(updatedUser, cancellationToken);
//         }
//         catch (Exception exception)
//         {
//             return new UserUnknownException(user.Id, exception);
//         }
//     }
// }