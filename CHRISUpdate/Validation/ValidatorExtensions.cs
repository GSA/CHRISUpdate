using FluentValidation;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace HRUpdate.Validation
{
    public static class ValidatorExtensions
    {
        public static IRuleBuilderOptions<T, TProperty> In<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder, params TProperty[] validOptions)
        {
            return ruleBuilder
                .Must(validOptions.Contains)
                .WithMessage("{PropertyName} submitted is not valid");
        }

        public static IRuleBuilderOptions<T, TProperty> ValidDate<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder)
        {
            DateTime date;
            return ruleBuilder
                .Must(e => DateTime.TryParse(e.ToString(), out date))
                .WithMessage("{PropertyName} must be a valid date");
        }

        public static IRuleBuilderOptions<T, TProperty> ValidPhone<T, TProperty>(this IRuleBuilder<T, TProperty> rulebuilder)
        {
            return rulebuilder
                 .Must(e => IsValidPhoneNumber_Alt(e.ToString()))
                 .WithMessage("{PropertyName} submitted is not valid");
        }

        private static bool IsValidPhoneNumber_Alt(string phoneNumber)
        {
            bool valid = Regex.IsMatch(phoneNumber, @"^[0-9]{3}[\/]{1}[0-9]{3}[-]{1}[0-9]{4}(([xX]){1}[0-9]{1,8}){0,1}$");

            if (!valid)
            {
                valid = Regex.IsMatch(phoneNumber, @"^\+[0-9]{1,3}\.[0-9]{4,14}(?:[xX][0-9]+)?$");
            }
            return valid;
        }
    }
}