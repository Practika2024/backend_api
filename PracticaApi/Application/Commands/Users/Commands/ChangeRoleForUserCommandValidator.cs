using FluentValidation;

namespace Application.Commands.Users.Commands;

public class ChangeRoleForUserCommandValidator : AbstractValidator<ChangeRoleForUserCommand>
{
    public ChangeRoleForUserCommandValidator()
    {
        RuleFor(command => command.UserId)
            .NotNull()
            .NotEmpty()
            .WithMessage("User ID cannot be null or empty.");

        RuleFor(command => command.RoleId)
            .NotNull()
            .NotEmpty()
            .WithMessage(command => $"User under ID: {command.UserId} must have role!");
    }
}