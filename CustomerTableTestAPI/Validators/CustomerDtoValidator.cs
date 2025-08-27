
using CustomerTableTest.Models.DTOs;
using FluentValidation;

public class CustomerDtoValidator : AbstractValidator<CustomerDto>
{
    public CustomerDtoValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Code is required")
            .Length(3, 10).WithMessage("Code must be between 3 and 10 characters");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(100).WithMessage("Name can't be longer than 100 characters");

        //RuleFor(x => x.PhoneNumber)
        //    .EmailAddress().WithMessage("Invalid phone format")
        //    .When(x => !string.IsNullOrEmpty(x.PhoneNumber));
    }
}
