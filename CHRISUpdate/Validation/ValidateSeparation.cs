using FluentValidation;
using FluentValidation.Results;
using HRUpdate.Lookups;
using HRUpdate.Mapping;
using HRUpdate.Models;
using HRUpdate.Process;
using System;
using System.Linq;

namespace HRUpdate.Validation
{
    internal class ValidateSeparation
    {
        private readonly Lookup lookups;
        private readonly HRMapper map = new HRMapper();

        public ValidateSeparation()
        {
            map.CreateLookupConfig();
            lookups = new LoadLookupData(map.CreateLookupMapping()).GetSeparationLookupData();
        }

        public ValidationResult ValidateSeparationInformation(Separation separationInformation)
        {
            SeparationValidator validator = new SeparationValidator(lookups);
            return validator.Validate(separationInformation);
        }
    }

    internal class SeparationValidator : AbstractValidator<Separation>
    {
        public SeparationValidator(Lookup lookups)
        {
            string[] separationTypes = lookups.separationLookup.Select(e => e.Code).ToArray();

            RuleFor(s => s.EmployeeID)
                .NotEmpty()
                .WithMessage($"{{PropertyName}} is required")
                .Length(1, 11)
                .WithMessage($"{{PropertyName}} length must be 1-11");
            RuleFor(s => s.SeparationCode)
                .NotEmpty()
                .WithMessage($"{{PropertyName}} is required")
                .Length(1, 11)
                .WithMessage($"{{PropertyName}} length must be 1-11")
                .In(separationTypes);
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