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
            //string formatted = string.Empty;

            //if (validOptions == null || validOptions.Length == 0)
            //{
            //    throw new ArgumentException("At least one valid option is expected", nameof(validOptions));
            //}
            //else if (validOptions.Length == 1)
            //{
            //    formatted = validOptions[0].ToString();
            //}
            //else
            //{
            //    // format like: option1, option2 or option3
            //    formatted = $"{string.Join(", ", validOptions.Select(vo => vo.ToString()).ToArray(), 0, validOptions.Length - 1)} or {validOptions.Last()}";
            //}

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
                //valid = libphonenumber.PhoneNumberUtil.Instance.IsPossibleNumber(phoneNumber, "US");
                //International phone number format: +CCC.NNNNNNNNNNNNxEEEE
                valid = Regex.IsMatch(phoneNumber, @"^\+[0-9]{1,3}\.[0-9]{4,14}(?:[xX][0-9]+)?$");
            }
            return valid;
        }

        /// <summary>
        /// Uses google libphonenumber api to validate phone number
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <returns></returns>
        //private static bool IsValidPhoneNumber(string phoneNumber)
        //{
        //    return libphonenumber.PhoneNumberUtil.Instance.IsPossibleNumber(phoneNumber, "US");
        //}
    }
}