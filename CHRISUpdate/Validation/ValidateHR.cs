﻿using FluentValidation;
using HRUpdate.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HRUpdate.Validation
{
    class ValidateHR
    {
        private readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public ValidateHR()
        {

        }

        private void ValidateEmployeeInformation(List<Employee> employeeInformation)
        {
            EmployeeValidator validator = new EmployeeValidator();
        }
    }

    class EmployeeValidator : AbstractValidator<Employee>
    {
        private string[] investigations = {"Temp", "Tier 1", "Tier 2", "Tier 3", "Tier 4", "Tier 5"}; 
        public EmployeeValidator()
        {
            ValidatorOptions.CascadeMode = CascadeMode.StopOnFirstFailure;

            #region Person
            RuleFor(Employee => Employee.Person.EmployeeID)
                .NotEmpty()
                .WithMessage("Employee id is required")
                .Length(1,11)
                .WithMessage("Employee id length must be 1-11");

            RuleFor(Employee => Employee.Person.FirstName)
                .Length(0, 60)
                .WithMessage("First name length must be 0-60");

            RuleFor(Employee => Employee.Person.LastName)
                .NotEmpty()
                .WithMessage("Last name is required")
                .Length(1, 60)
                .WithMessage("Last Name length must be 1-60");

            RuleFor(Employee => Employee.Person.MiddleName)
                .Length(0, 60)
                .WithMessage("Middle name must be 0-60");

            RuleFor(Employee => Employee.Person.Suffix)
                .Length(0, 15)
                .WithMessage("Suffix length must be 0-15");

            //Not submitted
            RuleFor(Employee => Employee.Person.CordialName)
                .Length(0, 24)
                .WithMessage("Cordial name length must be 0-24");

            RuleFor(Employee => Employee.Person.SocialSecurityNumber)
                .NotEmpty()
                .WithMessage("SSN is required")
                .Length(9)
                .WithMessage("SSN length must be 9");

            Unless(Employee => Employee.Person.Gender.Equals(""), () =>
            {
                RuleFor(Employee => Employee.Person.Gender)
                    .Matches(@"^[mfMF]{1}$")
                    .WithMessage("Gender must be 'M' or 'F'");
            });
            
            RuleFor(Employee => Employee.Person.ServiceComputationDateLeave)
                .Must(IsValidDate)
                .WithMessage("Service computation date leave must be a valid date");

            //FERO is nullable bool
            //RuleFor(Employee => Employee.Person.FederalEmergencyResponseOfficial)

            //LEO is nullable bool
            //RuleFor(Employee => Employee.Person.LawEnforcementOfficer)

            RuleFor(Employee => Employee.Person.Region)
                .Length(0, 3)
                .WithMessage("Region length must be 0-3");

            RuleFor(Employee => Employee.Person.OrganizationCode)
                .Length(0, 4)
                .WithMessage("Organization code length must be between 0-4");

            RuleFor(Employee => Employee.Person.JobTitle)
                .Length(0, 70)
                .WithMessage("Job title length must be 0-70");

            Unless(Employee => Employee.Person.HomeEmail.Equals(""), () =>
            {
                RuleFor(Employee => Employee.Person.HomeEmail)
                .Length(1, 64)
                .WithMessage("Home email must be between 1-64")
                .EmailAddress()
                .WithMessage("Home email must be a valid email address");
            });
            #endregion

            #region Address
            RuleFor(Employee => Employee.Address.HomeAddress1)
                .Length(0, 60)
                .WithMessage("Home address 1 length must be 0-60");

            RuleFor(Employee => Employee.Address.HomeAddress2)
                .Length(0, 60)
                .WithMessage("Home address 2 length must be 0-60");

            RuleFor(Employee => Employee.Address.HomeAddress3)
                .Length(0, 60)
                .WithMessage("Home address 3 length must be 0-60");

            RuleFor(Employee => Employee.Address.HomeCity)
                .Length(0, 50)
                .WithMessage("Home city length must be 0-50");

            Unless(E => E.Address.HomeState.Equals(""), () =>
            {
                RuleFor(Employee => Employee.Address.HomeState)
                .Matches(@"^[a-zA-Z]{2}$")
                .WithMessage("Home state must be A thru Z and 2 characters long");
            });
            
            RuleFor(Employee => Employee.Address.HomeZipCode)
                .Length(0, 10)
                .WithMessage("Home zip code length must be 0-10");

            Unless(Employee => Employee.Address.HomeCountry.Equals(""), ()=>
            {
                RuleFor(Employee => Employee.Address.HomeCountry)
                    .Matches(@"^[a-zA-Z]{2}$")
                    .WithMessage("Home country must be A thru Z and 2 characters long");                    
            });
            #endregion

            #region Birth
            RuleFor(Employee => Employee.Birth.CityOfBirth)
                .Length(0, 24)
                .WithMessage("City of birth length must be 0-24");

            Unless(Employee => Employee.Birth.StateOfBirth.Equals(""), () =>
            {
                RuleFor(Employee => Employee.Birth.StateOfBirth)
                    .Matches(@"^[a-zA-Z]{2}$")
                    .WithMessage("State of birth must be A thru Z and 2 characters long");
            });

            Unless(Employee => Employee.Birth.CountryOfBirth.Equals(""), () =>
            {
                RuleFor(Employee => Employee.Birth.CountryOfBirth)
                    .Matches(@"^[a-zA-Z]{2}$")
                    .WithMessage("Country of birth must be A thru Z and 2 characters long");                   
            });

            Unless(Employee => Employee.Birth.CountryOfCitizenship.Equals(""), () =>
            {
                RuleFor(Employee => Employee.Birth.CountryOfCitizenship)
                    .Matches(@"^[a-zA-Z]{2}$")
                    .WithMessage("Country of citizenship must be A thru Z and 2 characters long")
                    .Length(0, 2)
                    .WithMessage("Country of citizenship length must be 0-2");
            });

            //nullable bool
            //RuleFor(Employee => Employee.Birth.Citizen)

            Unless(Employee => Employee.Birth.DateOfBirth.Equals(null), () =>
            {
                RuleFor(Employee => Employee.Birth.DateOfBirth)
                    .Must(IsValidDate)
                    .WithMessage("Date of birth must be valid date");
            });
            #endregion

            #region Investigation
            RuleFor(Employee => Employee.Investigation.PriorInvestigation)
                .In(investigations)
                .Length(0, 20)
                .WithMessage("Prior investigation length must be 0-20");

            When(Employee => string.IsNullOrEmpty(Employee.Investigation.TypeOfInvestigation) || Employee.Investigation.DateOfInvestigation != null, () =>
            {
                RuleFor(Employee => Employee.Investigation.TypeOfInvestigation)
                    .NotEmpty()
                    .WithMessage("Type of investigation cannot be null when Date of investigation is not null")
                    .In(investigations)
                    .Length(1, 20)
                    .WithMessage("Type of investigation length must be 1-20");
                    
                RuleFor(Employee => Employee.Investigation.DateOfInvestigation)
                    .NotEmpty()
                    .WithMessage("Date of investigation cannot be null when Type of investigation is not null")
                    .Must(IsValidDate)
                    .WithMessage("Date of investigation must be a valid date");
            });
            
            RuleFor(Employee => Employee.Investigation.TypeOfInvestigationToRequest)
                .In(investigations)
                .Length(0, 12)
                .WithMessage("Type of investigation to request must be 0-12");

            When(Employee => Employee.Investigation.InitialResult != null || Employee.Investigation.InitialResultDate != null, () =>
            {
                RuleFor(Employee => Employee.Investigation.InitialResult)
                    .NotNull()
                    .WithMessage("Initial result cannot be null when Initial result date is not null");

                RuleFor(Employee => Employee.Investigation.InitialResultDate)
                    .NotNull()
                    .WithMessage("Initial result date cannot be null when initial result is not null")
                    .Must(IsValidDate)
                    .WithMessage("Initial result date must be a valid date");                    
            });

            When(Employee => Employee.Investigation.FinalResult != null || Employee.Investigation.FinalResultDate != null, () =>
            {
                RuleFor(Employee => Employee.Investigation.FinalResult)
                    .NotNull()
                    .WithMessage("Final result cannot be null when Final result date is not null");

                RuleFor(Employee => Employee.Investigation.FinalResultDate)
                    .NotNull()
                    .WithMessage("Final result date cannot be null when Final result is null")
                    .Must(IsValidDate)
                    .WithMessage("Final result date must be a valid date");                    
            });

            
            RuleFor(Employee => Employee.Investigation.AdjudicatorEmployeeID)
                .Length(0,11)
                .WithMessage("Adjudicators employee id length must be 0-11");
            #endregion

            #region Emergency
            RuleFor(Employee => Employee.Emergency.EmergencyContactName)
                .Length(0, 40)
                .WithMessage("Emergency contact name length must be 0-40");

            Unless(Employee => Employee.Emergency.EmergencyContactHomePhone.Equals(""), () =>
            {
                RuleFor(Employee => Employee.Emergency.EmergencyContactHomePhone)
                .Length(1, 24)
                .WithMessage("Emergency contact home phone length must be 0-10")
                .Must(IsValidPhoneNumber)
                .WithMessage("Emergency contact home phone must be a valid phone number");
            });            

            Unless(Employee => Employee.Emergency.EmergencyContactWorkPhone.Equals(""), () =>
            {
                RuleFor(Employee => Employee.Emergency.EmergencyContactWorkPhone)
                .Length(1, 24)
                .WithMessage("Emergency contact work phone length must be 0-10")
                .Must(IsValidPhoneNumber)
                .WithMessage("Emergency contact work phone must be a valid phone number");
            });            

            Unless(Employee => Employee.Emergency.EmergencyContactCellPhone.Equals(""), () =>
            {
                RuleFor(Employee => Employee.Emergency.EmergencyContactCellPhone)
                .Length(1, 24)
                .WithMessage("Emergency contact cell phone length must be 0-10")
                .Must(IsValidPhoneNumber)
                .WithMessage("Emergency contact cell phone must be a valid phone number");
            });
            
            RuleFor(Employee => Employee.Emergency.OutOfAreaContactName)
                .Length(0, 40)
                .WithMessage("Ouit of area contact name length must be 0-40");

            Unless(Employee => Employee.Emergency.OutOfAreaContactHomePhone.Equals(""), () =>
            {
                RuleFor(Employee => Employee.Emergency.OutOfAreaContactHomePhone)
                .Length(1, 24)
                .WithMessage("Out of area contact home phone length must be 0-10")
                .Must(IsValidPhoneNumber)
                .WithMessage("Out of area contact home phone must be a valid phone number");
            });
            
            Unless(Employee => Employee.Emergency.OutOfAreaContactWorkPhone.Equals(""), () =>
            {
                RuleFor(Employee => Employee.Emergency.OutOfAreaContactWorkPhone)
                .Length(1, 24)
                .WithMessage("Out of area contact work phone length must be 0-10")
                .Must(IsValidPhoneNumber)
                .WithMessage("Out of area contact work phone must be a valid phone number");
            });
            
            Unless(Employee => Employee.Emergency.OutOfAreaContactCellPhone.Equals(""), () =>
            {
                RuleFor(Employee => Employee.Emergency.OutOfAreaContactCellPhone)
                .Length(1, 24)
                .WithMessage("Out of area contact cell phone length must be 0-10")
                .Must(IsValidPhoneNumber)
                .WithMessage("Out of area contact cell phone must be a valid phone number");
            });
            #endregion

            #region Position
            RuleFor(Employee => Employee.Position.PositionControlNumber)
                .Length(0, 15)
                .WithMessage("Position control number length must be 0-15");

            RuleFor(Employee => Employee.Position.PositionTitle)
                .Length(0, 70)
                .WithMessage("Position title length must be 0-70");

            RuleFor(Employee => Employee.Position.PositionOrganization)
                .Length(0, 18)
                .WithMessage("Position organization length must be between 0-18");

            RuleFor(Employee => Employee.Position.SupervisoryStatus)
                .Length(0, 2)
                .WithMessage("Supervisory status length must be 0-2");

            RuleFor(Employee => Employee.Position.PayPlan)
                .Length(0, 3)
                .WithMessage("Pay plan length must be 0-3");

            RuleFor(Employee => Employee.Position.JobSeries)
                .Length(0, 8)
                .WithMessage("Job series length must be 0-8");

            RuleFor(Employee => Employee.Position.PayGrade)
                .Length(0, 3)
                .WithMessage("Pay grade length must be between 0-3");

            RuleFor(Employee => Employee.Position.WorkSchedule)
                .Length(0, 1)
                .WithMessage("Work schedule must be 0-1");

            //nullable bool
            //RuleFor(Employee => Employee.Position.PositionTeleworkEligibility)

            RuleFor(Employee => Employee.Position.PositionSensitivity)
                .Length(0, 4)
                .WithMessage("Position sensitivity length must be 0-4");

            RuleFor(Employee => Employee.Position.DutyLocationCode)
                .Length(0, 9)
                .WithMessage("Duty location code length must be 0-9");

            RuleFor(Employee => Employee.Position.DutyLocationCity)
                .Length(0, 40)
                .WithMessage("Duty location city length must be 0-40");

            Unless(Employee => Employee.Position.DutyLocationState.Equals(""), () =>
            {
                RuleFor(Employee => Employee.Position.DutyLocationState)
                    .Matches(@"^[a-zA-Z]{2}$")
                    .WithMessage("Duty location state must be A thru Z and 2 characters long");
            });
            
            RuleFor(Employee => Employee.Position.DutyLocationCounty)
                .Length(0, 40)
                .WithMessage("Duty location county must be 0-40");

            Unless(Employee => Employee.Position.PositionStartDate.Equals(null), () =>
            {
                RuleFor(Employee => Employee.Position.PositionStartDate)
                .Must(IsValidDate)
                .WithMessage("Position start date must be a valid date");
            });            

            RuleFor(Employee => Employee.Position.AgencyCodeSubelement)
                .Length(0, 4)
                .WithMessage("Agency code subelement");
            RuleFor(Employee => Employee.Position.SupervisorEmployeeID)
                .Length(0, 11)
                .WithMessage("Supervisor employee id length must be 0-11");
            #endregion

            #region Phone
            RuleFor(Employee => Employee.Phone.HomePhone)
                .Length(0, 24)
                .WithMessage("Home phone length must be 0-24")
                .Must(IsValidPhoneNumber)
                .WithMessage("Home phone must be a valid phone number");

            RuleFor(Employee => Employee.Phone.HomeCell)
                .Length(0, 24)
                .WithMessage("Home cell length must be 0-24")
                .Must(IsValidPhoneNumber)
                .WithMessage("Home cell must be a valid phone number");

            RuleFor(Employee => Employee.Phone.WorkPhone)
                .Length(0, 24)
                .WithMessage("Work phone length must be 0-24")
                .Must(IsValidPhoneNumber)
                .WithMessage("Work phone must be a valid phone number");

            RuleFor(Employee => Employee.Phone.WorkFax)
                .Length(0, 24)
                .WithMessage("Work fax length must be 0-24")
                .Must(IsValidPhoneNumber)
                .WithMessage("Work fax must be a valid phone number");

            RuleFor(Employee => Employee.Phone.WorkCell)
                .Length(0, 24)
                .WithMessage("Work cell length must be 0-24")
                .Must(IsValidPhoneNumber)
                .WithMessage("Work cell must be a valid phone number");

            RuleFor(Employee => Employee.Phone.WorkTextTelephone)
                .Length(0, 24)
                .WithMessage("Work text telephone length must be 0-24")
                .Must(IsValidPhoneNumber)
                .WithMessage("Work text telephone must be a valid phone number");
            #endregion

            //Detail - Not currently needed
        }

        /// <summary>
        /// Uses google libphonenumber api to validate phone number
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <returns></returns>
        public bool IsValidPhoneNumber(string phoneNumber)
        {
            return libphonenumber.PhoneNumberUtil.Instance.IsPossibleNumber(phoneNumber,"US");
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
                .WithMessage($"{{PropertyName}} must be one of these values: {formatted}");
        }
    }
}