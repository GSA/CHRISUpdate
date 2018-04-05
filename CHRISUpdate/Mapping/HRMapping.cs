using CsvHelper.Configuration;
using HRUpdate.Models;

namespace HRUpdate.Mapping
{
    public sealed class HRMapping : ClassMap<HR>
    {      
        public HRMapping()
        {
            //Affiliation is set in the poco class
            Map(m => m.EmployeeNumber).Index(HRConstants.CHRIS_EMPLOYEE_NUMBER);
            Map(m => m.ChrisID).Index(HRConstants.CHRIS_ID);
            Map(m => m.HomeAddress1).Index(HRConstants.HOME_ADDRESS_1);
            Map(m => m.HomeAddress2).Index(HRConstants.HOME_ADDRESS_2);
            Map(m => m.HomeAddress3).Index(HRConstants.HOME_ADDRESS_3);
            Map(m => m.HomeCity).Index(HRConstants.HOME_CITY);
            Map(m => m.HomeState).Index(HRConstants.HOME_STATE);
            Map(m => m.HomeZipCode).Index(HRConstants.HOME_ZIP_CODE);
            Map(m => m.HomeCountry).Index(HRConstants.HOME_COUNTRY);
            Map(m => m.SSN).Index(HRConstants.SSN); //.TypeConverter<SSNConverter>();

            References<EmployeeMap>(m => m.Employee);
            References<PositionMap>(m => m.Position);
            References<SecurityMap>(m => m.Security);
            References<SupervisorMap>(m => m.Supervisor);
            References<PersonMap>(m => m.Person);
        }
    }

    public sealed class EmployeeMap : ClassMap<Employee>
    {
        public EmployeeMap()
        {
            Map(m => m.AgencyCode).Index(HRConstants.AGENCY_CODE);
            Map(m => m.EmployeeID).Index(HRConstants.CHRIS_ID);
            Map(m => m.EmployeeStatus).Index(HRConstants.EMPLOYEE_STATUS);
            //Map(m => m.FamilySuffix).Index(CHRISConstants.EMPLOYEE_FAMILY_SUFFIX);            
            Map(m => m.FirstName).Index(HRConstants.EMPLOYEE_FIRST_NAME);
            Map(m => m.MiddleName).Index(HRConstants.EMPLOYEE_MIDDLE_NAME);
            Map(m => m.LastName).Index(HRConstants.EMPLOYEE_LAST_NAME);
            Map(m => m.Suffix).Index(HRConstants.EMPLOYEE_SUFFIX);
            Map(m => m.Gender).Index(HRConstants.GENDER);
            Map(m => m.UniqueEmployeeID).Index(HRConstants.CHRIS_EMPLOYEE_NUMBER);
            Map(m => m.SCDLeave).Index(HRConstants.SCD_LEAVE); //.TypeConverter<DateConverter>();
            Map(m => m.SupervisoryStatus).Index(HRConstants.SUPERVISORY_STATUS);
            //Map(m => m.SSN).Index(CHRISConstants.SSN);

            //Map(m => m.AssignmentStatus).Index(CHRISConstants.ASSIGNMENT_STATUS);
            //Map(m => m.DutyStatus).Index(CHRISConstants.DUTY_STATUS);
            //Map(m => m.TypeOfemployment).Index(CHRISConstants.TYPE_OF_EMPLOYMENT);
            //Map(m => m.Handicap).Index(CHRISConstants.HANDICAP);
        }
    }

    //Need two inserts 
    public sealed class PositionMap : ClassMap<Position>
    {
        public PositionMap()
        {
            Map(m => m.EmployeeID).Index(HRConstants.CHRIS_ID);
            Map(m => m.PositionTitle).Index(HRConstants.POSITION_TITLE);
            Map(m => m.PositionOrganization).Index(HRConstants.POSITION_ORGANIZATION);
            Map(m => m.AgencyCodeSubelment).Index(HRConstants.AGENCY_CODE_SUBELEMENT);
            Map(m => m.Sensitivity).Index(HRConstants.SENSITIVITY);
            Map(m => m.TeleworkEligible).Index(HRConstants.TELEWORK_ELIGIBLE);           

            //Map(m => m.IsDetail).Index(CHRISConstants.ASSIGNMENT_STATUS).TypeConverter<AssignmentConverter>();

            //Current Position Information

            Map(m => m.JobSeries).Index(HRConstants.JOB_SERIES);
            Map(m => m.JobTitle).Index(HRConstants.JOB_TITLE);
            Map(m => m.WorkSchedule).Index(HRConstants.WORK_SCHEDULE);
            Map(m => m.StartDate).Index(HRConstants.POSITION_START_DATE); //Just Added
            Map(m => m.LevelGrade).Index(HRConstants.LEVEL_GRADE);
            Map(m => m.OfficeSymbol).Index(HRConstants.POSITION_ORGANIZATION);
            Map(m => m.OrganizationCode).Index(HRConstants.ORGANIZATION_CODE);
            Map(m => m.Region).Index(HRConstants.REGION); //.TypeConverter<RegionConverter>(); //Look at using .Format();
            Map(m => m.PayPlan).Index(HRConstants.PAY_PLAN);
            Map(m => m.PositionControlNumber).Index(HRConstants.POSITION_CONTROL_NUMBER);
            Map(m => m.DutyCity).Index(HRConstants.DUTY_CITY);
            Map(m => m.DutyCode).Index(HRConstants.DUTY_CODE);
            Map(m => m.DutyCounty).Index(HRConstants.DUTY_COUNTY);
            Map(m => m.DutyState).Index(HRConstants.DUTY_STATE);

            //Detail Information

            Map(m => m.DetailBeginDate).Index(HRConstants.DETAIL_BEGIN_DATE);
            Map(m => m.DetailEndDate).Index(HRConstants.DETAIL_END_DATE);
            Map(m => m.DetailWorkSchedule).Index(HRConstants.DETAIL_WORK_SCHEDULE);
            Map(m => m.DetailLevelGrade).Index(HRConstants.DETAIL_LEVEL_GRADE);
            Map(m => m.DetailOfficeSymbol).Index(HRConstants.DETAIL_OFFICE_SYMBOL);
            Map(m => m.DetailOrganizationCode).Index(HRConstants.DETAIL_ORGANIZATION_CODE);
            Map(m => m.DetailRegion).Index(HRConstants.DETAIL_REGION); //.TypeConverter<RegionConverter>(); //Look at using .Format();
            Map(m => m.DetailPayPlan).Index(HRConstants.DETAIL_PAY_PLAN);
            Map(m => m.DetailPositionTitle).Index(HRConstants.DETAIL_POSITION_TITLE);
            Map(m => m.DetailPositionControlNumber).Index(HRConstants.DETAIL_POSITION_CONTROL_NUMBER);
            Map(m => m.DetailPositionNumber).Index(HRConstants.DETAIL_POSITION_CONTROL_NUMBER);
            Map(m => m.DetailDutyCity).Index(HRConstants.DETAIL_DUTY_CITY);
            Map(m => m.DetailDutyCode).Index(HRConstants.DETAIL_DUTY_CODE);
            Map(m => m.DetailDutyCounty).Index(HRConstants.DETAIL_DUTY_COUNTY);
            Map(m => m.DetailDutyState).Index(HRConstants.DETAIL_DUTY_STATE);
        }
    }

    //Need to handle this it's different (look up based on chris id) -- should just be the ID
    public sealed class SecurityMap : ClassMap<Security>
    {
        public SecurityMap()
        {
            Map(m => m.AdjudicationAuth).Index(HRConstants.ADJUDICATION_AUTH);
            Map(m => m.DateCompleted).Index(HRConstants.DATE_COMPLETED); //.TypeConverter<DateConverter>().Default(null);
            Map(m => m.EmployeeID).Index(HRConstants.CHRIS_ID);
            Map(m => m.PriorCompleted).Index(HRConstants.PRIOR_COMPLETED);
            Map(m => m.TypeCompleted).Index(HRConstants.TYPE_COMPLETED);
            Map(m => m.TypeRequested).Index(HRConstants.TYPE_REQUESTED);
        }
    }

    public sealed class SupervisorMap : ClassMap<Supervisor>
    {
        //Need to use HR Links employee's supervisor unique identifier column and get the name and ID of this person
        public SupervisorMap()
        {
            //CHRIS ID of current person
            Map(m => m.EmployeeID).Index(HRConstants.CHRIS_ID);
            Map(m => m.UniqueEmployeeID).Index(HRConstants.CHRIS_EMPLOYEE_NUMBER);
            Map(m => m.SupervisorEmployeeID).Index(HRConstants.SUPERVISOR_CHRIS_ID);        }
    }

    public sealed class PersonMap : ClassMap<Person>
    {
        public PersonMap()
        {
            Map(m => m.Gender).Index(HRConstants.GENDER);
            Map(m => m.SupervisoryLevel).Index(HRConstants.SUPERVISORY_STATUS);
            Map(m => m.ChrisID).Index(HRConstants.CHRIS_ID);
            Map(m => m.JobTitle).Index(HRConstants.JOB_TITLE);
            Map(m => m.OfficeSymbol).Index(HRConstants.POSITION_ORGANIZATION);
            //Regex is done in the SP
            Map(m => m.MajorOrg).Index(HRConstants.POSITION_ORGANIZATION);
            Map(m => m.Region).Index(HRConstants.REGION); //.TypeConverter<RegionConverter>(); //Look at using .Format();            
        }
    }
}
