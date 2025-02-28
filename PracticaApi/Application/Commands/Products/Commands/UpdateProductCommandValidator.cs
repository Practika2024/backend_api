using FluentValidation;

namespace Application.Commands.Products.Commands;

public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator()
    {
        RuleFor(p => p.Id)
            .NotEmpty().WithMessage("Product ID is required")
            .Must(id => id != Guid.Empty).WithMessage("Product ID cannot be empty");

        RuleFor(p => p.Name)
            .NotEmpty().WithMessage("Product name is required")
            .Must(name => name.Trim().Length > 0).WithMessage("Product name cannot be empty or whitespace")
            .MaximumLength(100).WithMessage("Product name cannot exceed 100 characters");

        RuleFor(p => p.TypeId)
            .NotEmpty().WithMessage("Type ID is required")
            .Must(typeId => typeId != Guid.Empty).WithMessage("Type ID cannot be empty");

        RuleFor(p => p.Description)
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters");

        RuleFor(p => p.ManufactureDate)
            .NotEmpty().WithMessage("Manufacture date is required")
            .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Manufacture date cannot be in the future");
    }
}