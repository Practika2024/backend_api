using FluentValidation;

namespace Application.Commands.Containers.Commands;

public class UpdateContainerCommandValidator : AbstractValidator<UpdateContainerCommand>
{
    public UpdateContainerCommandValidator()
    {
        RuleFor(c => c.Id)
            .NotEmpty().WithMessage("Container ID is required")
            .Must(id => id != Guid.Empty).WithMessage("Container ID cannot be empty");

        RuleFor(c => c.Name)
            .NotEmpty().WithMessage("Container name is required")
            .Must(name => name.Trim().Length > 0).WithMessage("Container name cannot be empty or whitespace")
            .MaximumLength(100).WithMessage("Container name cannot exceed 100 characters");

        RuleFor(c => c.Volume)
            .NotEmpty().WithMessage("Volume is required")
            .GreaterThan(0).WithMessage("Volume must be greater than 0");

        RuleFor(c => c.Notes)
            .MaximumLength(500).WithMessage("Notes cannot exceed 500 characters");
    }
}