using FluentValidation;
using FluentValidation.Results;
using HRUpdate.Models;

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

    }
}
