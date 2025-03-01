using Application.Commands.Authentications.Commands;
using FluentValidation;

namespace Application.Commands.Users.Commands;

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(u => u.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format");

        RuleFor(u => u.Password)
            .NotEmpty().WithMessage("Enter your password")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long");

        RuleFor(u => u.Name)
            .NotEmpty().WithMessage("Name is required")
            .Must(name => name.Trim().Length > 0).WithMessage("Name cannot be empty or whitespace")
            .MaximumLength(50).WithMessage("Name cannot exceed 50 characters");

        RuleFor(u => u.Surname)
            .MaximumLength(50).WithMessage("Surname cannot exceed 50 characters");

        RuleFor(u => u.Patronymic)
            .MaximumLength(50).WithMessage("Patronymic cannot exceed 50 characters");
    }
}