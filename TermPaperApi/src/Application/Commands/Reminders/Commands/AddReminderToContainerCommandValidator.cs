using FluentValidation;

namespace Application.Commands.Reminders.Commands;

public class AddReminderToContainerCommandValidator : AbstractValidator<AddReminderToContainerCommand>
{
    public AddReminderToContainerCommandValidator()
    {
        RuleFor(x => x.ContainerId)
            .NotEmpty().WithMessage("Container ID cannot be empty.");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title cannot be empty.")
            .MaximumLength(100).WithMessage("Title cannot exceed 100 characters.");

        RuleFor(x => x.DueDate)
            .NotEmpty().WithMessage("Due date cannot be empty.")
            .GreaterThan(DateTime.UtcNow).WithMessage("Due date must be in the future.");
    }
}