using FluentValidation;
using FluentValidation.Results;
using HRUpdate.Data;
using HRUpdate.Lookups;
using HRUpdate.Mapping;
using HRUpdate.Models;
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
            string[] separationTypes = lookups.separationLookup.Select(e => e.Code).Distinct().ToArray();

            RuleFor(s => s.EmployeeID)
                .NotEmpty()
                .WithMessage($"{{PropertyName}} is required");                
            RuleFor(s => s.SeparationCode)
                .NotEmpty()
                .WithMessage($"{{PropertyName}} is required")
                .Length(1, 11)
                .WithMessage($"{{PropertyName}} length must be 1-11")
                .In(separationTypes);
            RuleFor(s => s.SeparationDate)
                .NotNull()
                .WithMessage($"{{PropertyName}} should not be null")
                .ValidDate();
        }
    }
}