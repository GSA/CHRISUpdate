﻿using CsvHelper.Configuration;
using HRUpdate.Models;

namespace HRUpdate.Mapping
{
    public sealed class EmployeeMapping : ClassMap<Employee>
    {
        public EmployeeMapping()
        {
            References<PersonMap>(m => m.Person);
            References<AddressMap>(m => m.Address);
            References<BirthMap>(m => m.Birth);
            References<PositionMap>(m => m.Position);
            References<PhoneMap>(m => m.Phone);
            References<EmergencyMap>(m => m.Emergency);
        }
    }

    public sealed class PersonMap: ClassMap<Person>
    {
        public PersonMap()
        {
            Map(m => m.EmployeeID).Index(HRConstants.EMPLOYEE_NUMBER);
            Map(m => m.LastName).Index(HRConstants.EMPLOYEE_LAST_NAME);
            Map(m => m.Suffix).Index(HRConstants.EMPLOYEE_SUFFIX);
            Map(m => m.FirstName).Index(HRConstants.EMPLOYEE_FIRST_NAME);
            Map(m => m.MiddleName).Index(HRConstants.EMPLOYEE_MIDDLE_NAME);
            Map(m => m.CordialName).Index(HRConstants.CORDIAL_NAME);
            Map(m => m.SSN).Index(HRConstants.SSN);
            Map(m => m.Gender).Index(HRConstants.GENDER);
            Map(m => m.SCDLeave).Index(HRConstants.SCD_LEAVE);
            Map(m => m.FERO).Index(HRConstants.FERO).TypeConverter<FEROConverter>();
            Map(m => m.LEO).Index(HRConstants.LEO).TypeConverter<LEOConverter>(); ;
            Map(m => m.Region).Index(HRConstants.REGION);
            Map(m => m.OrganizationCode).Index(HRConstants.ORGANIZATION_CODE);
            Map(m => m.JobTitle).Index(HRConstants.JOB_TITLE);
            Map(m => m.HomeEmail).Index(HRConstants.PERSONAL_EMAIL_ADDRESS);
        }
    }
    public sealed class AddressMap : ClassMap<Address>
    {
        public AddressMap()
        {
            Map(m => m.HomeAddress1).Index(HRConstants.HOME_ADDRESS_1);
            Map(m => m.HomeAddress2).Index(HRConstants.HOME_ADDRESS_2);
            Map(m => m.HomeAddress2).Index(HRConstants.HOME_ADDRESS_2);
            Map(m => m.HomeCity).Index(HRConstants.HOME_CITY);
            Map(m => m.HomeState).Index(HRConstants.HOME_STATE);
            Map(m => m.HomeZipCode).Index(HRConstants.HOME_ZIP_CODE);
            Map(m => m.HomeCountry).Index(HRConstants.HOME_COUNTRY);
        }
    }

    public sealed class BirthMap : ClassMap<Birth>
    {
        public BirthMap()
        {
            Map(m => m.CityOfBirth).Index(HRConstants.BIRTH_CITY);
            Map(m => m.StateOfBirth).Index(HRConstants.BIRTH_STATE);
            Map(m => m.CountryOfBirth).Index(HRConstants.BIRTH_COUNTRY);
            Map(m => m.CountryOfCitizenship).Index(HRConstants.COUNTRY_OF_CITIZENSHIP);
            Map(m => m.Citizen).Index(HRConstants.CITIZEN);
            Map(m => m.DateOfBirth).Index(HRConstants.DATE_OF_BIRTH);

        }
    }

    public sealed class PositionMap: ClassMap<Position>
    {
        public PositionMap()
        {
            Map(m => m.PositionControlNumber).Index(HRConstants.POSITION_CONTROL_NUMBER);
            Map(m => m.PositionTitle).Index(HRConstants.POSITION_TITLE);
            Map(m => m.PositionOrganization).Index(HRConstants.POSITION_ORGANIZATION);
            Map(m => m.SupervisoryStatus).Index(HRConstants.SUPERVISORY_STATUS);
            Map(m => m.PayPlan).Index(HRConstants.PAY_PLAN);
            Map(m => m.JobSeries).Index(HRConstants.JOB_SERIES);
            Map(m => m.PayGrade).Index(HRConstants.LEVEL_GRADE);
            Map(m => m.WorkSchedule).Index(HRConstants.WORK_SCHEDULE);
            Map(m => m.PositionTeleworkEligibility).Index(HRConstants.TELEWORK_ELIGIBLE);
            Map(m => m.PositionStartDate).Index(HRConstants.POSITION_START_DATE);
            Map(m => m.DutyLocationCode).Index(HRConstants.DUTY_CODE);
            Map(m => m.DutyLocationCity).Index(HRConstants.DUTY_CITY);
            Map(m => m.DutyLocationState).Index(HRConstants.DUTY_STATE);
            Map(m => m.DutyLocationCounty).Index(HRConstants.DUTY_COUNTY);
            Map(m => m.AgencyCodeSubelement).Index(HRConstants.AGENCY_CODE_SUBELEMENT);
            Map(m => m.SupervisorEmployeeID).Index(HRConstants.SUPERVISOR_EMPLOYEE_ID);
        }
    }

    public sealed class PhoneMap: ClassMap<Phone>
    {
        public PhoneMap()
        {
            Map(m => m.HomePhone).Index(HRConstants.PERSONAL_HOME_PHONE);
            Map(m => m.HomeCell).Index(HRConstants.PERSONAL_CELL_PHONE);            
            Map(m => m.WorkPhone).Index(HRConstants.WORK_PHONE_NUMBER);
            Map(m => m.WorkFax).Index(HRConstants.WORK_FAX_NUMBER);
            Map(m => m.WorkCell).Index(HRConstants.WORK_CELL_NUMBER);
            Map(m => m.WorkTTY).Index(HRConstants.WORK_PHONE_NUMBER_TTY);
        }
    }

    public sealed class EmergencyMap: ClassMap<Emergency>
    {
        public EmergencyMap()
        {
            Map(m => m.EmergencyContactName).Index(HRConstants.EMERGENCY_POC_NAME);
            Map(m => m.EmergencyContactHomePhone).Index(HRConstants.EMERGENCY_POC_HOME_PHONE_NUMBER);
            Map(m => m.EmergencyContactWorkPhone).Index(HRConstants.EMERGENCY_POC_WORK_PHONE_NUMBER);
            Map(m => m.EmergencyContactCellPhone).Index(HRConstants.EMERGENCY_POC_CELL_NUMBER);
            Map(m => m.OutOfAreaContactName).Index(HRConstants.EMERGENCY_OUT_OF_AREA_NAME);
            Map(m => m.OutOfAreaContactHomePhone).Index(HRConstants.EMERGENCY_OUT_OF_AREA_HOME_PHONE_NUMBER);
            Map(m => m.OutOfAreaContactWorkPhone).Index(HRConstants.EMERGENCY_OUT_OF_AREA_HOME_WORK_PHONE_NUMBER);
            Map(m => m.OutOfAreaContactCellPhone).Index(HRConstants.EMERGENCY_OUT_OF_AREA_CELL_NUMBER);
        }
    }
}
