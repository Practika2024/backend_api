using FluentValidation;

namespace Application.Commands.Users.Commands;

public class ChangeRolesForUserCommandValidator : AbstractValidator<ChangeRolesForUserCommand>
{
    public ChangeRolesForUserCommandValidator()
    {
        RuleFor(command => command.UserId)
            .NotNull()
            .NotEmpty()
            .WithMessage("User ID cannot be null or empty.");

        RuleFor(command => command.Roles)
            .NotNull()
            .NotEmpty()
            .WithMessage(command => $"User under ID: {command.UserId} must have at least one role!");
    }
}