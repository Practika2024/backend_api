using FluentValidation;

namespace Application.Commands.Users.Commands;

public class DeleteUserCommandValidator : AbstractValidator<DeleteUserCommand>
{
    public DeleteUserCommandValidator()
    {
        RuleFor(command => command.UserId).NotNull().NotEmpty();
    }
}