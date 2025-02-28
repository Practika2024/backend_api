using Application.Commands.Authentications.Commands;
using FluentValidation;

namespace Application.Commands.Users.Commands;

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(u => u.Email)
            .NotEmpty().WithMessage("Enter your email address")
            .EmailAddress().WithMessage("Invalid mail format");

        RuleFor(u => u.Password)
            .NotEmpty().WithMessage("Enter your password")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long");

        RuleFor(u => u.Name)
            .NotEmpty().WithMessage("Enter your name")
            .Must(name => name.Trim().Length > 0).WithMessage("Name cannot be empty or whitespace");
    }
}