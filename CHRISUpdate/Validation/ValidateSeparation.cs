using FluentValidation;
using FluentValidation.Results;
using HRUpdate.Models;
using System;

namespace HRUpdate.Validation
{
    internal class ValidateSeparation
    {
        public ValidateSeparation() { }
        public ValidationResult ValidateSeparationInformation(Separation separationInformation)
        {
            SeparationValidator validator = new SeparationValidator();
            return validator.Validate(separationInformation);
        }
    }

    internal class SeparationValidator : AbstractValidator<Separation>
    {
        public SeparationValidator()
        {
            RuleFor(s => s.EmployeeID)
                .NotEmpty()
                .WithMessage($"{{PropertyName}} is required")
                .Length(1, 11)
                .WithMessage($"{{PropertyName}} length must be 1-11");
            RuleFor(s => s.SeparationCode)
                .NotEmpty()
                .WithMessage($"{{PropertyName}} is required")
                .Length(1, 11)
                .WithMessage($"{{PropertyName}} length must be 1-11");
            RuleFor(s => s.SeparationDate)
                .NotNull()
                .WithMessage($"{{PropertyName}} is not null")
                .ValidDate();
        }  
    }

    public static class SeparationValidatorExtensions
    {
        public static IRuleBuilderOptions<T, TProperty> ValidDate<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder)
        {
            DateTime date;
            return ruleBuilder
                .Must(e => DateTime.TryParse(e.ToString(), out date))
                .WithMessage($"{{PropertyName}} must be a valid date");
        }
    }
}
