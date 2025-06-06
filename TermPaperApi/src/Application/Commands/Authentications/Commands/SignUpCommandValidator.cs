﻿using FluentValidation;

namespace Application.Commands.Authentications.Commands;

public class SignUpCommandValidator : AbstractValidator<SignUpCommand>
{
    public SignUpCommandValidator()
    {
        RuleFor(u => u.Email)
            .NotEmpty().WithMessage("Enter your email address")
            .EmailAddress().WithMessage("Invalid mail format");

        RuleFor(u => u.Password)
            .NotEmpty().WithMessage("Enter your password")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long");

        RuleFor(u => u.Name)
            .NotEmpty().WithMessage("Name is required")
            .Must(name => name?.Trim().Length > 0).WithMessage("Name cannot be empty or whitespace")
            .MaximumLength(50).WithMessage("Name cannot exceed 50 characters");

        RuleFor(u => u.Surname)
            .MaximumLength(50).WithMessage("Surname cannot exceed 50 characters");

        RuleFor(u => u.Patronymic)
            .MaximumLength(50).WithMessage("Patronymic cannot exceed 50 characters");
    }
}