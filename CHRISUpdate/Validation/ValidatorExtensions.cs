using FluentValidation;
using System;
using System.Linq;

namespace HRUpdate.Validation
{
    public static class ValidatorExtensions
    {
        public static IRuleBuilderOptions<T, TProperty> In<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder, params TProperty[] validOptions)
        {
            string formatted;
            if (validOptions == null || validOptions.Length == 0)
            {
                throw new ArgumentException("At least one valid option is expected", nameof(validOptions));
            }
            else if (validOptions.Length == 1)
            {
                formatted = validOptions[0].ToString();
            }
            else
            {
                // format like: option1, option2 or option3
                formatted = $"{string.Join(", ", validOptions.Select(vo => vo.ToString()).ToArray(), 0, validOptions.Length - 1)} or {validOptions.Last()}";
            }

            return ruleBuilder
                .Must(validOptions.Contains)
                //.WithMessage($"{{PropertyName}} must be one of these values: {formatted}");
                .WithMessage("{PropertyName} submitted is not valid");
        }

        public static IRuleBuilderOptions<T, TProperty> ValidDate<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder)
        {
            DateTime date;
            return ruleBuilder
                .Must(e => DateTime.TryParse(e.ToString(), out date))
                .WithMessage("{PropertyName} must be a valid date");
        }
    }
}