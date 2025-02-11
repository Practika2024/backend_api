using FluentValidation;

namespace Application.Commands.Users.Commands;

public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(u => u.Email)
            .EmailAddress().WithMessage("Invalid mail format")
            .NotEmpty().WithMessage("Enter your email address");
        
        RuleFor(u=>u.Name).NotEmpty().WithMessage("Enter your name");
    }
}