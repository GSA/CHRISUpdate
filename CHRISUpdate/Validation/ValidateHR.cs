using FluentValidation;
using HRUpdate.Models;
using System;
using System.Collections.Generic;

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
        public EmployeeValidator()
        {
            ValidatorOptions.CascadeMode = CascadeMode.StopOnFirstFailure;

            //Person
            RuleFor(Employee => Employee.Person.EmployeeID)
                .NotEmpty()
                .WithMessage("Employee id is required")
                .Length(0,11)
                .WithMessage("Employee id length must be 0-11");
            RuleFor(Employee => Employee.Person.FirstName)
                .Length(0, 60)
                .WithMessage("First name length must be 0-60");
            RuleFor(Employee => Employee.Person.LastName)
                .NotEmpty()
                .WithMessage("Last name is required")
                .Length(0, 60)
                .WithMessage("Last Name length must be 0-60");
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
                .Length(0-9)
                .WithMessage("SSN length must be 0-9");
            RuleFor(Employee => Employee.Person.Gender)
                .Length(0-1)
                .WithMessage("Gender length must be 0-1");
            RuleFor(Employee => Employee.Person.ServiceComputationDateLeave)
                .Must(BeAValidDate)
                .WithMessage("Service computation date leave must be a valid date");
            //FERO is non nullable bool, nothing to check
            //RuleFor(Employee => Employee.Person.FederalEmergencyResponseOfficial)
            //LEO is non nullable bool, nothing to check
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
            RuleFor(Employee => Employee.Person.HomeEmail)
                .Length(0, 64)
                .WithMessage("Home email must be between 0-64");


            //Address
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
            RuleFor(Employee => Employee.Address.HomeState)
                .Length(0, 2)
                .WithMessage("Home state length must be 0-2");
            RuleFor(Employee => Employee.Address.HomeZipCode)
                .Length(0, 10)
                .WithMessage("Home zip code length must be 0-10");
            RuleFor(Employee => Employee.Address.HomeCountry)
                .Length(0, 2)
                .WithMessage("Home country must be 0-2");


            //Birth
            RuleFor(Employee => Employee.Birth.CityOfBirth)
                .Length(0, 24)
                .WithMessage("City of birth length must be 0-24");
            RuleFor(Employee => Employee.Birth.StateOfBirth)
                .Length(0, 2)
                .WithMessage("State of birth length must be 0-2");
            RuleFor(Employee => Employee.Birth.CountryOfBirth)
                .Length(0, 2)
                .WithMessage("Country of birth length must be 0-2");
            RuleFor(Employee => Employee.Birth.CountryOfCitizenship)
                .Length(0, 2)
                .WithMessage("Country of citizenship length must be 0-2");
            //Non nullable bool, nothing to check
            //RuleFor(Employee => Employee.Birth.Citizen)
            RuleFor(Employee => Employee.Birth.DateOfBirth)
                .Must(BeAValidDate)
                .WithMessage("Date of birth must be valid date");

            //Investigation
            RuleFor(Employee => Employee.Investigation.PriorInvestigation)
                .Length(0, 20)
                .WithMessage("Prior investigation length must be 0-20");
            RuleFor(Employee => Employee.Investigation.TypeOfInvestigation)
                .Length(0, 20)
                .WithMessage("Type of investigation length must be 0-20");
            RuleFor(Employee => Employee.Investigation.DateOfInvestigation)
                .Must(BeAValidDate)
                .WithMessage("Date of investigation must be a valid date");
            RuleFor(Employee => Employee.Investigation.TypeOfInvestigationToRequest)
                .Length(0, 12)
                .WithMessage("Type of investigation to request must be 0-12");

            When(Employee => Employee.Investigation.InitialResult != null || Employee.Investigation.InitialResultDate != null, () =>
            {
                RuleFor(Employee => Employee.Investigation.InitialResult)
                    .NotNull()
                    .WithMessage("Initial result cannot be null when Initial result date is not null");
                RuleFor(Employee => Employee.Investigation.InitialResultDate)
                    .Must(BeAValidDate)
                    .WithMessage("Initial result must be a valid date");
                RuleFor(Employee => Employee.Investigation.InitialResultDate)
                    .NotNull()
                    .WithMessage("Initial result date cannot be null when initial result is not null");
            });

            When(Employee => Employee.Investigation.FinalResult != null || Employee.Investigation.FinalResult != null, () =>
            {
                RuleFor(Employee => Employee.Investigation.FinalResult)
                    .NotNull()
                    .WithMessage("Final result cannot be null when Final result date is not null");
                RuleFor(Employee => Employee.Investigation.FinalResultDate)
                    .Must(BeAValidDate)
                    .WithMessage("Final result date must be a valid date");
                RuleFor(Employee => Employee.Investigation.FinalResultDate)
                    .NotNull()
                    .WithMessage("Final result date cannot be null when Final result is null");
            });

            
            RuleFor(Employee => Employee.Investigation.AdjudicatorEmployeeID)
                .Length(0,11)
                .WithMessage("Adjudicators employee id length must be 0-11");

            //Emergency
            RuleFor(Employee => Employee.Emergency.EmergencyContactName)
                .Length(0, 40)
                .WithMessage("Emergency contact name length must be 0-40");
            RuleFor(Employee => Employee.Emergency.EmergencyContactHomePhone)
                .Length(0, 10)
                .WithMessage("Emergency contact home phone length must be 0-10");
            RuleFor(Employee => Employee.Emergency.EmergencyContactWorkPhone)
                .Length(0, 10)
                .WithMessage("Emergency contact work phone length must be 0-10");
            RuleFor(Employee => Employee.Emergency.EmergencyContactCellPhone)
                .Length(0, 10)
                .WithMessage("Emergency contact cell phone length must be 0-10");
            RuleFor(Employee => Employee.Emergency.OutOfAreaContactName)
                .Length(0, 40)
                .WithMessage("Ouit of area contact name length must be 0-40");
            RuleFor(Employee => Employee.Emergency.OutOfAreaContactHomePhone)
                .Length(0, 10)
                .WithMessage("Out of area contact home phone length must be 0-10");
            RuleFor(Employee => Employee.Emergency.OutOfAreaContactWorkPhone)
                .Length(0, 10)
                .WithMessage("Out of area contact work phone length must be 0-10");
            RuleFor(Employee => Employee.Emergency.OutOfAreaContactCellPhone)
                .Length(0, 10)
                .WithMessage("Out of area contact cell phone length must be 0-10");

            //Position
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
            //Non nullable bool, nothing to check
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
            RuleFor(Employee => Employee.Position.DutyLocationState)
                .Length(0, 2)
                .WithMessage("Duty location state length must be 0-2");
            RuleFor(Employee => Employee.Position.DutyLocationCounty)
                .Length(0, 40)
                .WithMessage("Duty location county must be 0-40");
            RuleFor(Employee => Employee.Position.PositionStartDate)
                .Must(BeAValidDate)
                .WithMessage("Position start date must be a valid date");
            RuleFor(Employee => Employee.Position.AgencyCodeSubelement)
                .Length(0, 4)
                .WithMessage("Agency code subelement");
            RuleFor(Employee => Employee.Position.SupervisorEmployeeID)
                .Length(0, 11)
                .WithMessage("Supervisor employee id length must be 0-11");


            //Phone
            RuleFor(Employee => Employee.Phone.HomePhone)
                .Length(0, 24)
                .WithMessage("Home phone length must be 0-24");
            RuleFor(Employee => Employee.Phone.HomeCell)
                .Length(0, 24)
                .WithMessage("Home cell length must be 0-24");
            RuleFor(Employee => Employee.Phone.WorkPhone)
                .Length(0, 24)
                .WithMessage("Work phone length must be 0-24");
            RuleFor(Employee => Employee.Phone.WorkFax)
                .Length(0, 24)
                .WithMessage("Work fax length must be 0-24");
            RuleFor(Employee => Employee.Phone.WorkCell)
                .Length(0, 24)
                .WithMessage("Work cell length must be 0-24");
            RuleFor(Employee => Employee.Phone.WorkTextTelephone)
                .Length(0, 24)
                .WithMessage("Work text telephone length must be 0-24");


            //Detail
    }

        /// <summary>
        /// Checks if date given can be parsed into datetime
        /// </summary>
        /// <param name="date"></param>
        /// <returns>Bool</returns>
        public static bool BeAValidDate(DateTime? date)
        {
            DateTime _date;

            return DateTime.TryParse(date.ToString(), out _date);
        }
    }
}
