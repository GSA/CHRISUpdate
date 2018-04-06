using CsvHelper.Configuration;
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
            Map(m => m.FERO).Index(HRConstants.FERO);
            Map(m => m.LEO).Index(HRConstants.LEO);
            Map(m => m.Region).Index(HRConstants.REGION);
            Map(m => m.OrganizationCode).Index(HRConstants.ORGANIZATION_CODE);
            Map(m => m.JobTitle).Index(HRConstants.JOB_TITLE);
            Map(m => m.HomePhone).Index(HRConstants.PERSONAL_HOME_PHONE);
            Map(m => m.HomeCell).Index(HRConstants.PERSONAL_CELL_PHONE);
            Map(m => m.HomeEmail).Index(HRConstants.PERSONAL_EMAIL_ADDRESS);
            Map(m => m.WorkPhone).Index(HRConstants.WORK_PHONE_NUMBER);
            Map(m => m.WorkFax).Index(HRConstants.WORK_FAX_NUMBER);
            Map(m => m.WorkCell).Index(HRConstants.WORK_CELL_NUMBER);
            Map(m => m.WorkTTY).Index(HRConstants.WORK_PHONE_NUMBER_TTY);
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
}
