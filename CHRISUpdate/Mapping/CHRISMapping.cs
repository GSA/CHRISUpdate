using CHRISUpdate.Models;
using CsvHelper.Configuration;

namespace CHRISUpdate.Mapping
{
    public sealed class CHRISMapping : CsvClassMap<Chris>
    {      
        public CHRISMapping()
        {
            //Affiliation is set in the poco class
            Map(m => m.EmployeeNumber).Index(CHRISConstants.CHRIS_EMPLOYEE_NUMBER);
            Map(m => m.ChrisID).Index(CHRISConstants.CHRIS_ID);
            Map(m => m.HomeAddress1).Index(CHRISConstants.HOME_ADDRESS_1);
            Map(m => m.HomeAddress2).Index(CHRISConstants.HOME_ADDRESS_2);
            Map(m => m.HomeAddress3).Index(CHRISConstants.HOME_ADDRESS_3);
            Map(m => m.HomeCity).Index(CHRISConstants.HOME_CITY);
            Map(m => m.HomeState).Index(CHRISConstants.HOME_STATE);
            Map(m => m.HomeZipCode).Index(CHRISConstants.HOME_ZIP_CODE);
            Map(m => m.HomeCountry).Index(CHRISConstants.HOME_COUNTRY);
            Map(m => m.SSN).Index(CHRISConstants.SSN); //.TypeConverter<SSNConverter>();

            References<EmployeeMap>(m => m.Employee);
            References<PositionMap>(m => m.Position);
            References<SecurityMap>(m => m.Security);
            References<SupervisorMap>(m => m.Supervisor);
            References<PersonMap>(m => m.Person);
        }
    }

    public sealed class EmployeeMap : CsvClassMap<Employee>
    {
        public EmployeeMap()
        {
            Map(m => m.AgencyCode).Index(CHRISConstants.AGENCY_CODE);
            Map(m => m.AssignmentStatus).Index(CHRISConstants.ASSIGNMENT_STATUS);
            Map(m => m.DutyStatus).Index(CHRISConstants.DUTY_STATUS);
            Map(m => m.EmployeeID).Index(CHRISConstants.CHRIS_ID);
            Map(m => m.EmployeeStatus).Index(CHRISConstants.EMPLOYEE_STATUS);
            Map(m => m.FamilySuffix).Index(CHRISConstants.EMPLOYEE_FAMILY_SUFFIX);
            Map(m => m.FirstName).Index(CHRISConstants.EMPLOYEE_FIRST_NAME);
            Map(m => m.Gender).Index(CHRISConstants.GENDER);
            Map(m => m.MiddleName).Index(CHRISConstants.EMPLOYEE_MIDDLE_NAME);
            Map(m => m.TypeOfemployment).Index(CHRISConstants.TYPE_OF_EMPLOYMENT);
            Map(m => m.Handicap).Index(CHRISConstants.HANDICAP);
            Map(m => m.UniqueEmployeeID).Index(CHRISConstants.CHRIS_EMPLOYEE_NUMBER);
            Map(m => m.SCDLeave).Index(CHRISConstants.SCD_LEAVE); //.TypeConverter<DateConverter>();
            Map(m => m.SupervisoryStatus).Index(CHRISConstants.SUPERVISORY_STATUS);
            //Map(m => m.SSN).Index(CHRISConstants.SSN);
        }
    }

    //Need two inserts 
    public sealed class PositionMap : CsvClassMap<Position>
    {
        public PositionMap()
        {
            Map(m => m.EmployeeID).Index(CHRISConstants.CHRIS_ID);
            Map(m => m.PositionTitle).Index(CHRISConstants.POSITION_TITLE);
            Map(m => m.PositionOrganization).Index(CHRISConstants.POSITION_ORGANIZATION);
            Map(m => m.AgencyCodeSubelment).Index(CHRISConstants.AGENCY_CODE_SUBELEMENT);
            Map(m => m.Sensitivity).Index(CHRISConstants.SENSITIVITY);
            Map(m => m.TeleworkEligible).Index(CHRISConstants.TELEWORK_ELIGIBLE);

            Map(m => m.PositionNumber).Index(CHRISConstants.POSITION_NUMBER);

            Map(m => m.IsDetail).Index(CHRISConstants.ASSIGNMENT_STATUS).TypeConverter<AssignmentConverter>();

            //Current Position Information

            Map(m => m.JobSeries).Index(CHRISConstants.JOB_SERIES);
            Map(m => m.JobTitle).Index(CHRISConstants.JOB_TITLE);
            Map(m => m.WorkSchedule).Index(CHRISConstants.WORK_SCHEDULE);
            Map(m => m.StartDate).Index(CHRISConstants.POSITION_START_DATE); //Just Added
            Map(m => m.LevelGrade).Index(CHRISConstants.LEVEL_GRADE);
            Map(m => m.OfficeSymbol).Index(CHRISConstants.OFFICE_SYMBOL);
            Map(m => m.OrganizationCode).Index(CHRISConstants.ORGANIZATION_CODE);
            Map(m => m.Region).Index(CHRISConstants.REGION).TypeConverter<RegionConverter>(); //Look at using .Format();
            Map(m => m.PayPlan).Index(CHRISConstants.PAY_PLAN);
            Map(m => m.PositionControlNumber).Index(CHRISConstants.POSITION_CONTROL_NUMBER);
            Map(m => m.PositionControlNumberIndicator).Index(CHRISConstants.POSITION_CONTROL_NUMBER_INDICATOR);
            Map(m => m.DutyCity).Index(CHRISConstants.DUTY_CITY);
            Map(m => m.DutyCode).Index(CHRISConstants.DUTY_CODE);
            Map(m => m.DutyCounty).Index(CHRISConstants.DUTY_COUNTY);
            Map(m => m.DutyState).Index(CHRISConstants.DUTY_STATE);

            //Detail Information

            Map(m => m.DetailBeginDate).Index(CHRISConstants.DETAIL_BEGIN_DATE);
            Map(m => m.DetailEndDate).Index(CHRISConstants.DETAIL_END_DATE);
            Map(m => m.DetailWorkSchedule).Index(CHRISConstants.DETAIL_WORK_SCHEDULE);
            Map(m => m.DetailLevelGrade).Index(CHRISConstants.DETAIL_LEVEL_GRADE);
            Map(m => m.DetailOfficeSymbol).Index(CHRISConstants.DETAIL_OFFICE_SYMBOL);
            Map(m => m.DetailOrganizationCode).Index(CHRISConstants.DETAIL_ORGANIZATION_CODE);
            Map(m => m.DetailRegion).Index(CHRISConstants.DETAIL_REGION).TypeConverter<RegionConverter>(); //Look at using .Format();
            Map(m => m.DetailPayPlan).Index(CHRISConstants.DETAIL_PAY_PLAN);
            Map(m => m.DetailPositionTitle).Index(CHRISConstants.DETAIL_POSITION_TITLE);
            Map(m => m.DetailPositionControlNumber).Index(CHRISConstants.DETAIL_POSITION_CONTROL_NUMBER);
            Map(m => m.DetailPositionControlNumberIndicator).Index(CHRISConstants.DETAIL_POSITION_CONTROL_NUMBER_INDICATOR);
            Map(m => m.DetailPositionNumber).Index(CHRISConstants.DETAIL_POSTION_NUMBER);
            Map(m => m.DetailDutyCity).Index(CHRISConstants.DETAIL_DUTY_CITY);
            Map(m => m.DetailDutyCode).Index(CHRISConstants.DETAIL_DUTY_CODE);
            Map(m => m.DetailDutyCounty).Index(CHRISConstants.DETAIL_DUTY_COUNTY);
            Map(m => m.DetailDutyState).Index(CHRISConstants.DETAIL_DUTY_STATE);
        }
    }

    public sealed class SecurityMap : CsvClassMap<Security>
    {
        public SecurityMap()
        {
            Map(m => m.AdjudicationAuth).Index(CHRISConstants.ADJUDICATION_AUTH);
            Map(m => m.DateCompleted).Index(CHRISConstants.DATE_COMPLETED); //.TypeConverter<DateConverter>().Default(null);
            Map(m => m.EmployeeID).Index(CHRISConstants.CHRIS_ID);
            Map(m => m.PriorCompleted).Index(CHRISConstants.PRIOR_COMPLETED);
            Map(m => m.TypeCompleted).Index(CHRISConstants.TYPE_COMPLETED);
            Map(m => m.TypeRequested).Index(CHRISConstants.TYPE_REQUESTED);
        }
    }

    public sealed class SupervisorMap : CsvClassMap<Supervisor>
    {
        public SupervisorMap()
        {
            //CHRIS ID of current person
            Map(m => m.EmployeeID).Index(CHRISConstants.CHRIS_ID);

            Map(m => m.EMail).Index(CHRISConstants.SUPERVISOR_EMAIL);
            Map(m => m.FirstName).Index(CHRISConstants.SUPERVISOR_FIRST_NAME);
            Map(m => m.LastNameSuffix).Index(CHRISConstants.SUPERVISOR_LAST_NAME_SUFFIX);
            Map(m => m.MiddleName).Index(CHRISConstants.SUPERVISOR_MIDDLE_NAME);
            Map(m => m.PositionControlNumber).Index(CHRISConstants.SUPERVISOR_POSITION_CONTROL_NUMBER);
            Map(m => m.PositionControlNumberIndicator).Index(CHRISConstants.SUPERVISOR_POSITION_CONTROL_NUMBER_INDICATOR);
            Map(m => m.SupervisorEmployeeID).Index(CHRISConstants.SUPERVISOR_CHRIS_ID);
            Map(m => m.UniqueEmployeeID).Index(CHRISConstants.SUPERVISOR_EMPLOYEE_NUMBER);
        }
    }

    public sealed class PersonMap : CsvClassMap<Person>
    {
        public PersonMap()
        {
            Map(m => m.Gender).Index(CHRISConstants.GENDER);
            Map(m => m.SupervisoryLevel).Index(CHRISConstants.SUPERVISORY_STATUS);
            Map(m => m.ChrisID).Index(CHRISConstants.CHRIS_ID);
            Map(m => m.JobTitle).Index(CHRISConstants.JOB_TITLE);
            Map(m => m.OfficeSymbol).Index(CHRISConstants.OFFICE_SYMBOL);
            //Regex is done in the SP
            Map(m => m.MajorOrg).Index(CHRISConstants.OFFICE_SYMBOL);
            Map(m => m.Region).Index(CHRISConstants.REGION).TypeConverter<RegionConverter>(); //Look at using .Format();

            //Do not map this will have to be set before saving
            //Map(m => m.Supervisor).ToString();
        }
    }
}
