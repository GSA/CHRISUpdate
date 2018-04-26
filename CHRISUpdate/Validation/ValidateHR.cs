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
    internal class ValidateHR
    {
        private readonly Lookup lookups;
        private HRMapper map;

        public ValidateHR()
        {
            map.CreateLookupConfig();
            lookups = new LoadLookupData(map.CreateLookupMapping()).GetEmployeeLookupData();
        }

        public ValidationResult ValidateEmployeeInformation(Employee employeeInformation)
        {
            EmployeeValidator validator = new EmployeeValidator(lookups);

            return validator.Validate(employeeInformation);
        }
    }

    internal class EmployeeValidator : AbstractValidator<Employee>
    {
        public EmployeeValidator(Lookup lookups)
        {
            string[] investigationTypes = lookups.investigationLookup.Select(e => e.Tier).ToArray();

            #region Person

            RuleFor(Employee => Employee.Person.EmployeeID)
                .NotEmpty()
                .WithMessage($"{{PropertyName}} is required")
                .Length(1, 11)
                .WithMessage($"{{PropertyName}} length must be 1-11");

            RuleFor(Employee => Employee.Person.FirstName)
                .Length(0, 60)
                .WithMessage($"{{PropertyName}} length must be 0-60");

            RuleFor(Employee => Employee.Person.LastName)
                .NotEmpty()
                .WithMessage($"{{PropertyName}} is required")
                .Length(1, 60)
                .WithMessage($"{{PropertyName}} length must be 1-60");

            RuleFor(Employee => Employee.Person.MiddleName)
                .Length(0, 60)
                .WithMessage($"{{PropertyName}} length must be 0-60");

            RuleFor(Employee => Employee.Person.Suffix)
                .Length(0, 15)
                .WithMessage($"{{PropertyName}} length must be 0-15");

            //Not submitted
            RuleFor(Employee => Employee.Person.CordialName)
                .Length(0, 24)
                .WithMessage($"{{PropertyName}} length must be 0-24");

            RuleFor(Employee => Employee.Person.SocialSecurityNumber)
                .NotEmpty()
                .WithMessage($"{{PropertyName}} is required")
                .Length(9)
                .WithMessage($"{{PropertyName}} length must be 9");

            Unless(Employee => string.IsNullOrEmpty(Employee.Person.Gender), () =>
            {
                RuleFor(Employee => Employee.Person.Gender)
                    .Matches(@"^[mfMF]{1}$")
                    .WithMessage($"{{PropertyName}} must be 'M' or 'F'");
            });

            Unless(e => e.Person.ServiceComputationDateLeave.Equals(null), () =>
            {
                RuleFor(Employee => Employee.Person.ServiceComputationDateLeave)
                    .Must(IsValidDate)
                    .WithMessage($"{{PropertyName}} must be a valid date");
            });

            //FERO is nullable bool
            //RuleFor(Employee => Employee.Person.FederalEmergencyResponseOfficial)

            //LEO is nullable bool
            //RuleFor(Employee => Employee.Person.LawEnforcementOfficer)

            RuleFor(Employee => Employee.Person.Region)
                .Length(0, 3)
                .WithMessage($"{{PropertyName}} length must be 0-3");

            RuleFor(Employee => Employee.Person.OrganizationCode)
                .Length(0, 4)
                .WithMessage($"{{PropertyName}} length must be between 0-4");

            RuleFor(Employee => Employee.Person.JobTitle)
                .Length(0, 70)
                .WithMessage($"{{PropertyName}} length must be 0-70");

            Unless(Employee => string.IsNullOrEmpty(Employee.Person.HomeEmail), () =>
            {
                RuleFor(Employee => Employee.Person.HomeEmail)
                .Length(1, 64)
                .WithMessage($"{{PropertyName}}l must be between 1-64")
                .EmailAddress()
                .WithMessage($"{{PropertyName}} must be a valid email address");
            });

            #endregion Person

            #region Address

            RuleFor(Employee => Employee.Address.HomeAddress1)
                .Length(0, 60)
                .WithMessage($"{{PropertyName}} length must be 0-60");

            RuleFor(Employee => Employee.Address.HomeAddress2)
                .Length(0, 60)
                .WithMessage($"{{PropertyName}} length must be 0-60");

            RuleFor(Employee => Employee.Address.HomeAddress3)
                .Length(0, 60)
                .WithMessage($"{{PropertyName}} length must be 0-60");

            RuleFor(Employee => Employee.Address.HomeCity)
                .Length(0, 50)
                .WithMessage($"{{PropertyName}} length must be 0-50");

            Unless(E => string.IsNullOrEmpty(E.Address.HomeState), () =>
            {
                RuleFor(Employee => Employee.Address.HomeState)
                    .Matches(@"^[a-zA-Z]{2}$")
                    .WithMessage($"{{PropertyName}} must be A though Z and 2 characters long");
            });

            RuleFor(Employee => Employee.Address.HomeZipCode)
                .Length(0, 10)
                .WithMessage($"{{PropertyName}} length must be 0-10");

            Unless(Employee => string.IsNullOrEmpty(Employee.Address.HomeCountry), () =>
            {
                RuleFor(Employee => Employee.Address.HomeCountry)
                    .Matches(@"^[a-zA-Z]{2}$")
                    .WithMessage($"{{PropertyName}} must be A though Z and 2 characters long");
            });

            #endregion Address

            #region Birth

            RuleFor(Employee => Employee.Birth.CityOfBirth)
                .Length(0, 24)
                .WithMessage($"{{PropertyName}} length must be 0-24");

            Unless(Employee => string.IsNullOrEmpty(Employee.Birth.StateOfBirth), () =>
            {
                RuleFor(Employee => Employee.Birth.StateOfBirth)
                    .Matches(@"^[a-zA-Z]{2}$")
                    .WithMessage($"{{PropertyName}} must be A though Z and 2 characters long");
            });

            Unless(Employee => string.IsNullOrEmpty(Employee.Birth.CountryOfBirth), () =>
            {
                RuleFor(Employee => Employee.Birth.CountryOfBirth)
                    .Matches(@"^[a-zA-Z]{2}$")
                    .WithMessage($"{{PropertyName}}h must be A though Z and 2 characters long");
            });

            Unless(Employee => string.IsNullOrEmpty(Employee.Birth.CountryOfCitizenship), () =>
            {
                RuleFor(Employee => Employee.Birth.CountryOfCitizenship)
                    .Matches(@"^[a-zA-Z]{2}$")
                    .WithMessage($"{{PropertyName}} must be A though Z and 2 characters long")
                    .Length(0, 2)
                    .WithMessage($"{{PropertyName}} length must be 0-2");
            });

            //nullable bool
            //RuleFor(Employee => Employee.Birth.Citizen)

            Unless(Employee => Employee.Birth.DateOfBirth.Equals(null), () =>
            {
                RuleFor(Employee => Employee.Birth.DateOfBirth)
                    .Must(IsValidDate)
                    .WithMessage($"{{PropertyName}} must be valid date");
            });

            #endregion Birth

            #region Investigation

            Unless(e => string.IsNullOrEmpty(e.Investigation.PriorInvestigation), () =>
              {
                  RuleFor(Employee => Employee.Investigation.PriorInvestigation)
                      .In(investigationTypes)
                      .Length(1, 20)
                      .WithMessage($"{{PropertyName}} length must be 1-20");
              });

            Unless(Employee => string.IsNullOrEmpty(Employee.Investigation.TypeOfInvestigation) || Employee.Investigation.DateOfInvestigation == null, () =>
            {
                RuleFor(Employee => Employee.Investigation.TypeOfInvestigation)
                    .NotEmpty()
                    .WithMessage($"{{PropertyName}} cannot be null when Date of investigation is not null")
                    .In(investigationTypes)
                    .Length(1, 20)
                    .WithMessage($"{{PropertyName}} length must be 1-20");

                RuleFor(Employee => Employee.Investigation.DateOfInvestigation)
                    .NotEmpty()
                    .WithMessage($"{{PropertyName}}n cannot be null when Type of investigation is not null")
                    .Must(IsValidDate)
                    .WithMessage($"{{PropertyName}} must be a valid date");
            });

            Unless(e => string.IsNullOrEmpty(e.Investigation.TypeOfInvestigationToRequest), () =>
            {
                RuleFor(Employee => Employee.Investigation.TypeOfInvestigationToRequest)
                    .In(investigationTypes)
                    .Length(0, 12)
                    .WithMessage($"{{PropertyName}} length must be 0-12");
            });

            When(Employee => Employee.Investigation.InitialResult != null || Employee.Investigation.InitialResultDate != null, () =>
            {
                RuleFor(Employee => Employee.Investigation.InitialResult)
                    .NotNull()
                    .WithMessage($"{{PropertyName}} cannot be null when Initial result date is not null");

                RuleFor(Employee => Employee.Investigation.InitialResultDate)
                    .NotNull()
                    .WithMessage($"{{PropertyName}} cannot be null when initial result is not null")
                    .Must(IsValidDate)
                    .WithMessage($"{{PropertyName}} must be a valid date");
            });

            When(Employee => Employee.Investigation.FinalResult != null || Employee.Investigation.FinalResultDate != null, () =>
            {
                RuleFor(Employee => Employee.Investigation.FinalResult)
                    .NotNull()
                    .WithMessage($"{{PropertyName}} cannot be null when Final result date is not null");

                RuleFor(Employee => Employee.Investigation.FinalResultDate)
                    .NotNull()
                    .WithMessage($"{{PropertyName}} cannot be null when Final result is not null")
                    .Must(IsValidDate)
                    .WithMessage($"{{PropertyName}} must be a valid date");
            });

            RuleFor(Employee => Employee.Investigation.AdjudicatorEmployeeID)
                .Length(0, 11)
                .WithMessage($"{{PropertyName}} length must be 0-11");

            #endregion Investigation

            #region Emergency

            RuleFor(Employee => Employee.Emergency.EmergencyContactName)
                .Length(0, 40)
                .WithMessage($"{{PropertyName}} length must be 0-40");

            Unless(Employee => string.IsNullOrEmpty(Employee.Emergency.EmergencyContactHomePhone), () =>
            {
                RuleFor(Employee => Employee.Emergency.EmergencyContactHomePhone)
                .Length(1, 24)
                .WithMessage($"{{PropertyName}} length must be 1-24")
                .Must(IsValidPhoneNumber)
                .WithMessage($"{{PropertyName}} must be a valid phone number");
            });

            Unless(Employee => string.IsNullOrEmpty(Employee.Emergency.EmergencyContactWorkPhone), () =>
            {
                RuleFor(Employee => Employee.Emergency.EmergencyContactWorkPhone)
                .Length(1, 24)
                .WithMessage($"{{PropertyName}} length must be 1-24")
                .Must(IsValidPhoneNumber)
                .WithMessage($"{{PropertyName}} must be a valid phone number");
            });

            Unless(Employee => string.IsNullOrEmpty(Employee.Emergency.EmergencyContactCellPhone), () =>
            {
                RuleFor(Employee => Employee.Emergency.EmergencyContactCellPhone)
                .Length(1, 24)
                .WithMessage($"{{PropertyName}} length must be 1-24")
                .Must(IsValidPhoneNumber)
                .WithMessage($"{{PropertyName}} must be a valid phone number");
            });

            RuleFor(Employee => Employee.Emergency.OutOfAreaContactName)
                .Length(0, 40)
                .WithMessage($"{{PropertyName}} length must be 0-40");

            Unless(Employee => string.IsNullOrEmpty(Employee.Emergency.OutOfAreaContactHomePhone), () =>
            {
                RuleFor(Employee => Employee.Emergency.OutOfAreaContactHomePhone)
                .Length(1, 24)
                .WithMessage($"{{PropertyName}} length must be 1-24")
                .Must(IsValidPhoneNumber)
                .WithMessage($"{{PropertyName}} must be a valid phone number");
            });

            Unless(Employee => string.IsNullOrEmpty(Employee.Emergency.OutOfAreaContactWorkPhone), () =>
            {
                RuleFor(Employee => Employee.Emergency.OutOfAreaContactWorkPhone)
                .Length(1, 24)
                .WithMessage($"{{PropertyName}} length must be 1-24")
                .Must(IsValidPhoneNumber)
                .WithMessage($"{{PropertyName}} must be a valid phone number");
            });

            Unless(Employee => string.IsNullOrEmpty(Employee.Emergency.OutOfAreaContactCellPhone), () =>
            {
                RuleFor(Employee => Employee.Emergency.OutOfAreaContactCellPhone)
                .Length(1, 24)
                .WithMessage($"{{PropertyName}} length must be 1-24")
                .Must(IsValidPhoneNumber)
                .WithMessage($"{{PropertyName}} must be a valid phone number");
            });

            #endregion Emergency

            #region Position

            RuleFor(Employee => Employee.Position.PositionControlNumber)
                .Length(0, 15)
                .WithMessage($"{{PropertyName}} length must be 0-15");

            RuleFor(Employee => Employee.Position.PositionTitle)
                .Length(0, 70)
                .WithMessage($"{{PropertyName}} length must be 0-70");

            RuleFor(Employee => Employee.Position.PositionOrganization)
                .Length(0, 18)
                .WithMessage($"{{PropertyName}} length must be between 0-18");

            RuleFor(Employee => Employee.Position.SupervisoryStatus)
                .Length(0, 2)
                .WithMessage($"{{PropertyName}} length must be 0-2");

            RuleFor(Employee => Employee.Position.PayPlan)
                .Length(0, 3)
                .WithMessage($"{{PropertyName}} length must be 0-3");

            RuleFor(Employee => Employee.Position.JobSeries)
                .Length(0, 8)
                .WithMessage($"{{PropertyName}} length must be 0-8");

            RuleFor(Employee => Employee.Position.PayGrade)
                .Length(0, 3)
                .WithMessage($"{{PropertyName}} length must be between 0-3");

            RuleFor(Employee => Employee.Position.WorkSchedule)
                .Length(0, 1)
                .WithMessage($"{{PropertyName}} must be 0-1");

            //nullable bool
            //RuleFor(Employee => Employee.Position.PositionTeleworkEligibility)

            RuleFor(Employee => Employee.Position.PositionSensitivity)
                .Length(0, 4)
                .WithMessage($"{{PropertyName}} length must be 0-4");

            RuleFor(Employee => Employee.Position.DutyLocationCode)
                .Length(0, 9)
                .WithMessage($"{{PropertyName}} length must be 0-9");

            RuleFor(Employee => Employee.Position.DutyLocationCity)
                .Length(0, 40)
                .WithMessage($"{{PropertyName}} length must be 0-40");

            Unless(Employee => string.IsNullOrEmpty(Employee.Position.DutyLocationState), () =>
            {
                RuleFor(Employee => Employee.Position.DutyLocationState)
                    .Matches(@"^[a-zA-Z]{2}$")
                    .WithMessage($"{{PropertyName}} must be A though Z and 2 characters long");
            });

            RuleFor(Employee => Employee.Position.DutyLocationCounty)
                .Length(0, 40)
                .WithMessage($"{{PropertyName}} must be 0-40");

            Unless(Employee => Employee.Position.PositionStartDate.Equals(null), () =>
            {
                RuleFor(Employee => Employee.Position.PositionStartDate)
                .Must(IsValidDate)
                .WithMessage($"{{PropertyName}} must be a valid date");
            });

            RuleFor(Employee => Employee.Position.AgencyCodeSubelement)
                .Length(0, 4)
                .WithMessage($"{{PropertyName}} length must be 0-4");

            RuleFor(Employee => Employee.Position.SupervisorEmployeeID)
                .Length(0, 11)
                .WithMessage($"{{PropertyName}} length must be 0-11");

            #endregion Position

            #region Phone

            Unless(e => string.IsNullOrEmpty(e.Phone.HomePhone), () =>
            {
                RuleFor(Employee => Employee.Phone.HomePhone)
                .Length(0, 24)
                .WithMessage($"{{PropertyName}} length must be 0-24")
                .Must(IsValidPhoneNumber)
                .WithMessage($"{{PropertyName}} must be a valid phone number");
            });

            Unless(e => string.IsNullOrEmpty(e.Phone.HomeCell), () =>
            {
                RuleFor(Employee => Employee.Phone.HomeCell)
                .Length(0, 24)
                .WithMessage($"{{PropertyName}} length must be 0-24")
                .Must(IsValidPhoneNumber)
                .WithMessage($"{{PropertyName}} must be a valid phone number");
            });

            Unless(e => string.IsNullOrEmpty(e.Phone.WorkPhone), () =>
            {
                RuleFor(Employee => Employee.Phone.WorkPhone)
                .Length(0, 24)
                .WithMessage($"{{PropertyName}} length must be 0-24")
                .Must(IsValidPhoneNumber)
                .WithMessage($"{{PropertyName}} must be a valid phone number");
            });

            Unless(e => string.IsNullOrEmpty(e.Phone.WorkFax), () =>
            {
                RuleFor(Employee => Employee.Phone.WorkFax)
                .Length(0, 24)
                .WithMessage($"{{PropertyName}} length must be 0-24")
                .Must(IsValidPhoneNumber)
                .WithMessage($"{{PropertyName}} must be a valid phone number");
            });

            Unless(e => string.IsNullOrEmpty(e.Phone.WorkCell), () =>
            {
                RuleFor(Employee => Employee.Phone.WorkCell)
                .Length(0, 24)
                .WithMessage($"{{PropertyName}} length must be 0-24")
                .Must(IsValidPhoneNumber)
                .WithMessage($"{{PropertyName}} must be a valid phone number");
            });

            Unless(e => string.IsNullOrEmpty(e.Phone.WorkTextTelephone), () =>
            {
                RuleFor(Employee => Employee.Phone.WorkTextTelephone)
                .Length(0, 24)
                .WithMessage($"{{PropertyName}} length must be 0-24")
                .Must(IsValidPhoneNumber)
                .WithMessage($"{{PropertyName}} must be a valid phone number");
            });

            #endregion Phone

            //Detail - Not currently needed
        }

        /// <summary>
        /// Uses google libphonenumber api to validate phone number
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <returns></returns>
        public bool IsValidPhoneNumber(string phoneNumber)
        {
            return libphonenumber.PhoneNumberUtil.Instance.IsPossibleNumber(phoneNumber, "US");
        }

        /// <summary>
        /// Checks if date given can be parsed into datetime
        /// </summary>
        /// <param name="date"></param>
        /// <returns>Bool</returns>
        public bool IsValidDate(DateTime? date)
        {
            DateTime _date;
            return DateTime.TryParse(date.ToString(), out _date);
        }
    }

    public static class EmployeeValidatorExtensions
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
                .WithMessage($"{{PropertyName}} must be one of these values: {formatted}");
        }
    }
}