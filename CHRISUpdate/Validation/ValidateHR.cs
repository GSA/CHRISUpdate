using FluentValidation;
using FluentValidation.Results;
using HRUpdate.Data;
using HRUpdate.Lookups;
using HRUpdate.Mapping;
using HRUpdate.Models;
using System.Collections.Generic;
using System.Linq;

namespace HRUpdate.Validation
{
    internal class ValidateHR
    {
        private readonly Dictionary<string, string[]> lookups = new Dictionary<string, string[]>();
        private readonly HRMapper map = new HRMapper();

        public ValidateHR()
        {
            map.CreateLookupConfig();
            Lookup lookup = new LoadLookupData(map.CreateLookupMapping()).GetEmployeeLookupData();
            lookups.Add("InvestigationTypes", lookup.investigationLookup.Select(e => e.Tier).Distinct().ToArray());
            lookups.Add("StateCodes", lookup.stateLookup.Select(s => s.Code).Distinct().ToArray());
            lookups.Add("CountryCodes", lookup.countryLookup.Select(c => c.Code).Distinct().ToArray());
        }

        public ValidationResult ValidateEmployeeCriticalInfo(Employee employeeInformation)
        {
            EmployeeCriticalErrorValidator validator = new EmployeeCriticalErrorValidator(lookups);

            return validator.Validate(employeeInformation);
        }
    }

    internal class EmployeeCriticalErrorValidator : AbstractValidator<Employee>
    {
        public EmployeeCriticalErrorValidator(Dictionary<string, string[]> lookups)
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;

            #region Person

            //**********PERSON***********************************************************************************************
            RuleFor(Employee => Employee.Person.EmployeeID)
                .NotEmpty()
                .WithMessage($"{{PropertyName}} is required")
                .MaximumLength(11)
                .WithMessage($"{{PropertyName}} length must be 0-11");

            RuleFor(Employee => Employee.Person.FirstName)
                .NotEmpty()
                .WithMessage($"{{PropertyName}} is required")
                .MaximumLength(60)
                .WithMessage($"{{PropertyName}} length must be 0-60");

            RuleFor(Employee => Employee.Person.LastName)
                .NotEmpty()
                .WithMessage($"{{PropertyName}} is required")
                .MaximumLength(60)
                .WithMessage($"{{PropertyName}} length must be 0-60");

            RuleFor(Employee => Employee.Person.MiddleName)
                .MaximumLength(60)
                .WithMessage($"{{PropertyName}} length must be 0-60");

            RuleFor(Employee => Employee.Person.Suffix)
                .MaximumLength(15)
                .WithMessage($"{{PropertyName}} length must be 0-15");

            //Not submitted
            //RuleFor(Employee => Employee.Person.CordialName)
            //    .MaximumLength(24)
            //    .WithMessage($"{{PropertyName}} length must be 0-24");

            RuleFor(Employee => Employee.Person.SocialSecurityNumber)
                .NotEmpty()
                .WithMessage($"{{PropertyName}} is required")
                .Length(9)
                .WithMessage($"{{PropertyName}} length must be 9");

            Unless(e => string.IsNullOrEmpty(e.Person.Gender), () =>
            {
                RuleFor(Employee => Employee.Person.Gender)
                    .Matches(@"^[mfMF]{1}$")
                    .WithMessage($"{{PropertyName}} must be one of these values: 'M', 'm', 'F', 'f'");
            });

            Unless(e => e.Person.ServiceComputationDateLeave.Equals(null), () =>
            {
                RuleFor(Employee => Employee.Person.ServiceComputationDateLeave)
                    .ValidDate();
            });

            //RuleFor(Employee => Employee.Person.FederalEmergencyResponseOfficial)
            //    .NotNull()
            //    .WithMessage($"{{PropertyName}} is required");

            //RuleFor(Employee => Employee.Person.LawEnforcementOfficer)
            //    .NotNull()
            //    .WithMessage($"{{PropertyName}} is required");

            RuleFor(Employee => Employee.Person.Region)
                //.NotEmpty()
                //.WithMessage($"{{PropertyName}} is required")
                .MaximumLength(3)
                .WithMessage($"{{PropertyName}} length must be 0-3");

            RuleFor(Employee => Employee.Person.OrganizationCode)
                //.NotEmpty()
                //.WithMessage($"{{PropertyName}} is required")
                .MaximumLength(4)
                .WithMessage($"{{PropertyName}} length must be 0-4");

            RuleFor(Employee => Employee.Person.JobTitle)
                .MaximumLength(70)
                .WithMessage($"{{PropertyName}} length must be 0-70");

            RuleFor(Employee => Employee.Person.HomeEmail)
                //.NotEmpty()
                //.WithMessage($"{{PropertyName}} is required")
                .MaximumLength(64)
                .WithMessage($"{{PropertyName}}l must be between 0-64");

            Unless(e => string.IsNullOrEmpty(e.Person.HomeEmail), () =>
            {
                RuleFor(Employee => Employee.Person.HomeEmail)
                    .EmailAddress()
                    .WithMessage($"{{PropertyName}} must be a valid email address");
            });

            #endregion Person

            #region Address

            //***************************Address*******************************************************************
            RuleFor(Employee => Employee.Address.HomeAddress1)
                //.NotEmpty()
                //.WithMessage($"{{PropertyName}} is required")
                .MaximumLength(60)
                .WithMessage($"{{PropertyName}} length must be 0-60");

            RuleFor(Employee => Employee.Address.HomeAddress2)
                .MaximumLength(60)
                .WithMessage($"{{PropertyName}} length must be 0-60");

            RuleFor(Employee => Employee.Address.HomeAddress3)
                .MaximumLength(60)
                .WithMessage($"{{PropertyName}} length must be 0-60");

            RuleFor(Employee => Employee.Address.HomeCity)
                //.NotEmpty()
                //.WithMessage($"{{PropertyName}} is required")
                .MaximumLength(50)
                .WithMessage($"{{PropertyName}} length must be 0-50");

            Unless(e => string.IsNullOrEmpty(e.Address.HomeCountry), () =>
            {
                When(e => e.Address.HomeCountry.ToLower().Equals("us") ||
                    e.Address.HomeCountry.ToLower().Equals("ca") ||
                    e.Address.HomeCountry.ToLower().Equals("mx"), () =>
                    {
                        Unless(e => string.IsNullOrEmpty(e.Address.HomeState), () =>
                        {
                            RuleFor(Employee => Employee.Address.HomeState)
                                //.NotEmpty()
                                //.WithMessage($"{{PropertyName}} is required")
                                .In(lookups["StateCodes"]);
                        });
                    });
            });

            RuleFor(Employee => Employee.Address.HomeZipCode)
                .MaximumLength(10)
                .WithMessage($"{{PropertyName}} length must be 0-10");

            Unless(e => string.IsNullOrEmpty(e.Address.HomeCountry), () =>
            {
                RuleFor(Employee => Employee.Address.HomeCountry)
                //.NotEmpty()
                //.WithMessage($"{{PropertyName}} is required")
                .In(lookups["CountryCodes"]);
            });

            #endregion Address

            #region Birth

            //******************************Birth***********************************************************************
            Unless(e => string.IsNullOrEmpty(e.Birth.CityOfBirth), () =>
            {
                RuleFor(Employee => Employee.Birth.CityOfBirth)
                //.NotEmpty()
                //.WithMessage($"{{PropertyName}} is required")
                .MaximumLength(24)
                .WithMessage($"{{PropertyName}} length must be 0-24");
            });

            Unless(e => string.IsNullOrEmpty(e.Birth.CountryOfBirth), () =>
            {
                When(e => e.Birth.CountryOfBirth.ToLower().Equals("us") ||
                    e.Address.HomeCountry.ToLower().Equals("ca") ||
                    e.Address.HomeCountry.ToLower().Equals("mx"), () =>
                    {
                        Unless(e => string.IsNullOrEmpty(e.Birth.StateOfBirth), () =>
                        {
                            RuleFor(Employee => Employee.Birth.StateOfBirth)
                                //.NotEmpty()
                                //.WithMessage($"{{PropertyName}} is required")
                                .In(lookups["StateCodes"]);
                        });
                    });
            });

            Unless(e => string.IsNullOrEmpty(e.Birth.CountryOfBirth), () =>
            {
                RuleFor(Employee => Employee.Birth.CountryOfBirth)
                //.NotEmpty()
                //.WithMessage($"{{PropertyName}} is required")
                .In(lookups["CountryCodes"]);
            });

            Unless(e => string.IsNullOrEmpty(e.Birth.CountryOfCitizenship), () =>
            {
                RuleFor(Employee => Employee.Birth.CountryOfCitizenship)
                //.NotEmpty()
                //.WithMessage($"{{PropertyName}} is required")
                .In(lookups["CountryCodes"]);
            });

            //RuleFor(Employee => Employee.Birth.Citizen)
            //    .NotNull()
            //    .WithMessage($"{{PropertyName}} is required");

            Unless(e => e.Birth.DateOfBirth.Equals(null), () =>
            {
                RuleFor(Employee => Employee.Birth.DateOfBirth)
                //.NotNull()
                //.WithMessage($"{{PropertyName}} is required")
                .ValidDate()
                .WithMessage($"{{PropertyName}} must be valid date");
            });

            #endregion Birth

            #region Investigation

            //**********INVESTIGATION**************************************************************************

            Unless(e => string.IsNullOrEmpty(e.Investigation.PriorInvestigation), () =>
            {
                RuleFor(Employee => Employee.Investigation.PriorInvestigation)
                    .In(lookups["InvestigationTypes"])
                    .MaximumLength(20)
                    .WithMessage($"{{PropertyName}} length must be 0-20");
            });

            Unless(Employee => string.IsNullOrEmpty(Employee.Investigation.TypeOfInvestigation) &&
                        Employee.Investigation.DateOfInvestigation == null, () =>
                        {
                            RuleFor(Employee => Employee.Investigation.TypeOfInvestigation)
                                .NotEmpty()
                                .WithMessage($"{{PropertyName}} cannot be null when Date of investigation is not null")
                                .In(lookups["InvestigationTypes"])
                                .MaximumLength(20)
                                .WithMessage($"{{PropertyName}} length must be 0-20");

                            RuleFor(Employee => Employee.Investigation.DateOfInvestigation)
                                .NotEmpty()
                                .WithMessage($"{{PropertyName}}n cannot be null when Type of investigation is not null")
                                .ValidDate()
                                .WithMessage($"{{PropertyName}} must be a valid date");
                        });

            Unless(e => string.IsNullOrEmpty(e.Investigation.TypeOfInvestigationToRequest), () =>
            {
                RuleFor(Employee => Employee.Investigation.TypeOfInvestigationToRequest)
                    .In(lookups["InvestigationTypes"])
                    .MaximumLength(12)
                    .WithMessage($"{{PropertyName}} length must be 0-12");
            });

            When(Employee => Employee.Investigation.InitialResult != null && Employee.Investigation.InitialResultDate != null, () =>
            {
                RuleFor(Employee => Employee.Investigation.InitialResult)
                    .NotNull()
                    .WithMessage($"{{PropertyName}} cannot be null when Initial result date is not null");

                RuleFor(Employee => Employee.Investigation.InitialResultDate)
                    .NotNull()
                    .WithMessage($"{{PropertyName}} cannot be null when initial result is not null")
                    .ValidDate()
                    .WithMessage($"{{PropertyName}} must be a valid date");
            });

            When(Employee => Employee.Investigation.FinalResult != null && Employee.Investigation.FinalResultDate != null, () =>
            {
                RuleFor(Employee => Employee.Investigation.FinalResult)
                    .NotNull()
                    .WithMessage($"{{PropertyName}} cannot be null when Final result date is not null");

                RuleFor(Employee => Employee.Investigation.FinalResultDate)
                    .NotNull()
                    .WithMessage($"{{PropertyName}} cannot be null when Final result is not null")
                    .ValidDate()
                    .WithMessage($"{{PropertyName}} must be a valid date");
            });

            RuleFor(Employee => Employee.Investigation.AdjudicatorEmployeeID)
                .MaximumLength(11)
                .WithMessage($"{{PropertyName}} length must be 0-11");

            #endregion Investigation

            #region Emergency

            //***********EMERGENCY*******************************************************************************
            RuleFor(Employee => Employee.Emergency.EmergencyContactName)
                .MaximumLength(40)
                .WithMessage($"{{PropertyName}} length must be 0-40");

            Unless(Employee => string.IsNullOrEmpty(Employee.Emergency.EmergencyContactHomePhone), () =>
            {
                RuleFor(Employee => Employee.Emergency.EmergencyContactHomePhone)
                .MaximumLength(24)
                .WithMessage($"{{PropertyName}} length must be 0-24")
                .ValidPhone()
                .WithMessage($"{{PropertyName}} must be a valid phone number");
            });

            Unless(Employee => string.IsNullOrEmpty(Employee.Emergency.EmergencyContactWorkPhone), () =>
            {
                RuleFor(Employee => Employee.Emergency.EmergencyContactWorkPhone)
                .MaximumLength(24)
                .WithMessage($"{{PropertyName}} length must be 0-24")
                .ValidPhone()
                .WithMessage($"{{PropertyName}} must be a valid phone number");
            });

            Unless(Employee => string.IsNullOrEmpty(Employee.Emergency.EmergencyContactCellPhone), () =>
            {
                RuleFor(Employee => Employee.Emergency.EmergencyContactCellPhone)
                .MaximumLength(24)
                .WithMessage($"{{PropertyName}} length must be 0-24")
                .ValidPhone()
                .WithMessage($"{{PropertyName}} must be a valid phone number");
            });

            RuleFor(Employee => Employee.Emergency.OutOfAreaContactName)
                .MaximumLength(40)
                .WithMessage($"{{PropertyName}} length must be 0-40");

            Unless(Employee => string.IsNullOrEmpty(Employee.Emergency.OutOfAreaContactHomePhone), () =>
            {
                RuleFor(Employee => Employee.Emergency.OutOfAreaContactHomePhone)
                .MaximumLength(24)
                .WithMessage($"{{PropertyName}} length must be 0-24")
                .ValidPhone()
                .WithMessage($"{{PropertyName}} must be a valid phone number");
            });

            Unless(Employee => string.IsNullOrEmpty(Employee.Emergency.OutOfAreaContactWorkPhone), () =>
            {
                RuleFor(Employee => Employee.Emergency.OutOfAreaContactWorkPhone)
                .MaximumLength(24)
                .WithMessage($"{{PropertyName}} length must be 0-24")
                .ValidPhone()
                .WithMessage($"{{PropertyName}} must be a valid phone number");
            });

            Unless(Employee => string.IsNullOrEmpty(Employee.Emergency.OutOfAreaContactCellPhone), () =>
            {
                RuleFor(Employee => Employee.Emergency.OutOfAreaContactCellPhone)
                .MaximumLength(24)
                .WithMessage($"{{PropertyName}} length must be 0-24")
                .ValidPhone()
                .WithMessage($"{{PropertyName}} must be a valid phone number");
            });

            #endregion Emergency

            #region Position

            //**********POSITION******************************************************************************************
            RuleFor(Employee => Employee.Position.PositionControlNumber)
                .MaximumLength(15)
                .WithMessage($"{{PropertyName}} length must be 0-15");

            //RuleFor(Employee => Employee.Position.PositionTitle)
            //    .MaximumLength(70)
            //    .WithMessage($"{{PropertyName}} length must be 0-70");

            RuleFor(Employee => Employee.Position.PositionOrganization)
                .MaximumLength(18)
                .WithMessage($"{{PropertyName}} length must be between 0-18");

            RuleFor(Employee => Employee.Position.SupervisoryStatus)
                .MaximumLength(2)
                .WithMessage($"{{PropertyName}} length must be 0-2");

            RuleFor(Employee => Employee.Position.PayPlan)
                .MaximumLength(3)
                .WithMessage($"{{PropertyName}} length must be 0-3");

            RuleFor(Employee => Employee.Position.JobSeries)
                .MaximumLength(8)
                .WithMessage($"{{PropertyName}} length must be 0-8");

            RuleFor(Employee => Employee.Position.PayGrade)
                .MaximumLength(3)
                .WithMessage($"{{PropertyName}} length must be between 0-3");

            RuleFor(Employee => Employee.Position.WorkSchedule)
                .MaximumLength(1)
                .WithMessage($"{{PropertyName}} must be 0-1");

            //nullable bool
            //RuleFor(Employee => Employee.Position.PositionTeleworkEligibility)

            RuleFor(Employee => Employee.Position.PositionSensitivity)
                .MaximumLength(4)
                .WithMessage($"{{PropertyName}} length must be 0-4");

            RuleFor(Employee => Employee.Position.DutyLocationCode)
                .MaximumLength(9)
                .WithMessage($"{{PropertyName}} length must be 0-9");

            RuleFor(Employee => Employee.Position.DutyLocationCity)
                .MaximumLength(40)
                .WithMessage($"{{PropertyName}} length must be 0-40");

            Unless(Employee => string.IsNullOrEmpty(Employee.Position.DutyLocationState), () =>
            {
                RuleFor(Employee => Employee.Position.DutyLocationState)
                    .In(lookups["StateCodes"]);
            });

            RuleFor(Employee => Employee.Position.DutyLocationCounty)
                .MaximumLength(40)
                .WithMessage($"{{PropertyName}} must be 0-40");

            Unless(Employee => Employee.Position.PositionStartDate.Equals(null), () =>
            {
                RuleFor(Employee => Employee.Position.PositionStartDate)
                .ValidDate();
            });

            RuleFor(Employee => Employee.Position.AgencyCodeSubelement)
                .MaximumLength(4)
                .WithMessage($"{{PropertyName}} length must be 0-4");

            //RuleFor(Employee => Employee.Position.SupervisorEmployeeID)
            //    .MaximumLength(11)
            //    .WithMessage($"{{PropertyName}} length must be 0-11");

            #endregion Position

            #region Phone

            //**********PHONE*****************************************************************************************
            Unless(e => string.IsNullOrEmpty(e.Phone.HomePhone), () =>
            {
                RuleFor(Employee => Employee.Phone.HomePhone)
                .MaximumLength(24)
                .WithMessage($"{{PropertyName}} length must be 0-24")
                .ValidPhone()
                .WithMessage($"{{PropertyName}} must be a valid phone number");
            });

            Unless(e => string.IsNullOrEmpty(e.Phone.HomeCell), () =>
            {
                RuleFor(Employee => Employee.Phone.HomeCell)
                .MaximumLength(24)
                .WithMessage($"{{PropertyName}} length must be 0-24")
                .ValidPhone()
                .WithMessage($"{{PropertyName}} must be a valid phone number");
            });

            Unless(e => string.IsNullOrEmpty(e.Phone.WorkPhone), () =>
            {
                RuleFor(Employee => Employee.Phone.WorkPhone)
                .MaximumLength(24)
                .WithMessage($"{{PropertyName}} length must be 0-24")
                .ValidPhone()
                .WithMessage($"{{PropertyName}} must be a valid phone number");
            });

            Unless(e => string.IsNullOrEmpty(e.Phone.WorkFax), () =>
            {
                RuleFor(Employee => Employee.Phone.WorkFax)
                .MaximumLength(24)
                .WithMessage($"{{PropertyName}} length must be 0-24")
                .ValidPhone()
                .WithMessage($"{{PropertyName}} must be a valid phone number");
            });

            Unless(e => string.IsNullOrEmpty(e.Phone.WorkCell), () =>
            {
                RuleFor(Employee => Employee.Phone.WorkCell)
                .MaximumLength(24)
                .WithMessage($"{{PropertyName}} length must be 0-24")
                .ValidPhone()
                .WithMessage($"{{PropertyName}} must be a valid phone number");
            });

            Unless(e => string.IsNullOrEmpty(e.Phone.WorkTextTelephone), () =>
            {
                RuleFor(Employee => Employee.Phone.WorkTextTelephone)
                .MaximumLength(24)
                .WithMessage($"{{PropertyName}} length must be 0-24")
                .ValidPhone()
                .WithMessage($"{{PropertyName}} must be a valid phone number");
            });

            #endregion Phone

            //Detail - Not currently needed
        }
    }
}