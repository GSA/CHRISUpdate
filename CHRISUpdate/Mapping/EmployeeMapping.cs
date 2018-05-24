using AutoMapper;
using CsvHelper.Configuration;
using HRUpdate.Data;
using HRUpdate.Lookups;
using HRUpdate.Models;
using System.Collections.Generic;

namespace HRUpdate.Mapping
{
    public sealed class EmployeeMapping : ClassMap<Employee>
    {
        public EmployeeMapping()
        {
            Lookup lookups;
            HRMapper map = new HRMapper();
            IMapper lookupMapper;

            map.CreateLookupConfig();

            lookupMapper = map.CreateLookupMapping();

            LoadLookupData loadLookupData = new LoadLookupData(lookupMapper);

            lookups = loadLookupData.GetEmployeeLookupData();

            References<PersonMap>(r => r.Person);
            References<AddressMap>(r => r.Address, lookups.stateLookup, lookups.countryLookup);
            References<BirthMap>(r => r.Birth, lookups.stateLookup, lookups.countryLookup);
            References<PositionMap>(r => r.Position, lookups.stateLookup);
            References<PhoneMap>(r => r.Phone);
            References<EmergencyMap>(r => r.Emergency);
            References<InvestigationMap>(r => r.Investigation, lookups.investigationLookup);

            //At this time we will not be handling Detail information
            //References<DetailMap>(r => r.Detail, lookups.stateLookup);
        }
    }

    public sealed class PersonMap : ClassMap<Person>
    {
        public PersonMap()
        {
            Map(m => m.EmployeeID).Index(HRConstants.EMPLOYEE_NUMBER);
            Map(m => m.LastName).Index(HRConstants.EMPLOYEE_LAST_NAME);
            Map(m => m.Suffix).Index(HRConstants.EMPLOYEE_SUFFIX);
            Map(m => m.FirstName).Index(HRConstants.EMPLOYEE_FIRST_NAME);
            Map(m => m.MiddleName).Index(HRConstants.EMPLOYEE_MIDDLE_NAME);
            Map(m => m.SocialSecurityNumber).Index(HRConstants.SOCIAL_SECURITY_NUMBER);
            Map(m => m.Gender).Index(HRConstants.GENDER);
            Map(m => m.ServiceComputationDateLeave).Index(HRConstants.SERVICE_COMPUTATION_DATE_LEAVE);
            Map(m => m.FederalEmergencyResponseOfficial).Index(HRConstants.FEDERAL_EMERGENCY_RESPONSE_OFFICIAL).TypeConverter<FederalEmergencyResponseOfficialConverter>();
            Map(m => m.LawEnforcementOfficer).Index(HRConstants.LAW_ENFORCEMENT_OFFICER).TypeConverter<LawEnforcementOfficerConverter>();
            Map(m => m.Region).Index(HRConstants.REGION).TypeConverter<RegionConverter>();
            Map(m => m.MajorOrg).Index(HRConstants.POSITION_ORGANIZATION).TypeConverter<MajorOrgConverter>();
            Map(m => m.JobTitle).Index(HRConstants.POSITION_TITLE);
            Map(m => m.HomeEmail).Index(HRConstants.PERSONAL_EMAIL_ADDRESS);
        }
    }

    public sealed class AddressMap : ClassMap<Address>
    {
        public AddressMap(List<StateLookup> stateLookup, List<CountryLookup> countryLookup)
        {
            var stateCodeConverter = new StateCodeConverter(stateLookup);
            var countryCodeConverter = new CountryCodeConverter(countryLookup);

            Map(m => m.HomeAddress1).Index(HRConstants.HOME_ADDRESS_1);
            Map(m => m.HomeAddress2).Index(HRConstants.HOME_ADDRESS_2);
            Map(m => m.HomeAddress3).Index(HRConstants.HOME_ADDRESS_3);
            Map(m => m.HomeCity).Index(HRConstants.HOME_CITY);
            Map(m => m.HomeState).Index(HRConstants.HOME_STATE).TypeConverter(stateCodeConverter);
            Map(m => m.HomeZipCode).Index(HRConstants.HOME_ZIP_CODE);
            Map(m => m.HomeCountry).Index(HRConstants.HOME_COUNTRY).TypeConverter(countryCodeConverter);
        }
    }

    public sealed class BirthMap : ClassMap<Birth>
    {
        public BirthMap(List<StateLookup> stateLookup, List<CountryLookup> countryLookup)
        {
            var stateCodeConverter = new StateCodeConverter(stateLookup);
            var countryCodeConverter = new CountryCodeConverter(countryLookup);

            Map(m => m.CityOfBirth).Index(HRConstants.BIRTH_CITY);
            Map(m => m.StateOfBirth).Index(HRConstants.BIRTH_STATE).TypeConverter(stateCodeConverter);
            Map(m => m.CountryOfBirth).Index(HRConstants.BIRTH_COUNTRY).TypeConverter(countryCodeConverter);
            Map(m => m.CountryOfCitizenship).Index(HRConstants.COUNTRY_OF_CITIZENSHIP).TypeConverter(countryCodeConverter);
            Map(m => m.Citizen).Index(HRConstants.CITIZEN);
            Map(m => m.DateOfBirth).Index(HRConstants.DATE_OF_BIRTH).TypeConverter<DateConverter>();
        }
    }

    public sealed class PositionMap : ClassMap<Position>
    {
        public PositionMap(List<StateLookup> stateLookup)
        {
            var stateCodeConverter = new StateCodeConverter(stateLookup);

            Map(m => m.PositionControlNumber).Index(HRConstants.POSITION_CONTROL_NUMBER);
            //Map(m => m.PositionTitle).Index(HRConstants.POSITION_TITLE);
            Map(m => m.PositionOrganization).Index(HRConstants.POSITION_ORGANIZATION);
            Map(m => m.SupervisoryStatus).Index(HRConstants.SUPERVISORY_STATUS);
            Map(m => m.PayPlan).Index(HRConstants.PAY_PLAN);
            Map(m => m.JobSeries).Index(HRConstants.JOB_SERIES);
            Map(m => m.PayGrade).Index(HRConstants.LEVEL_GRADE);
            Map(m => m.WorkSchedule).Index(HRConstants.WORK_SCHEDULE);
            Map(m => m.PositionTeleworkEligibility).Index(HRConstants.POSITION_TELEWORK_ELIGIBLE).TypeConverter<PositionTeleworkEligibilityConverter>();
            Map(m => m.PositionSensitivity).Index(HRConstants.POSITION_SENSITIVITY);
            Map(m => m.PositionStartDate).Index(HRConstants.POSITION_START_DATE).TypeConverter<DateConverter>();
            Map(m => m.DutyLocationCode).Index(HRConstants.DUTY_LOCATION_CODE);
            Map(m => m.DutyLocationCity).Index(HRConstants.DUTY_LOCATION_CITY);
            Map(m => m.DutyLocationState).Index(HRConstants.DUTY_LOCATION_STATE).TypeConverter(stateCodeConverter);
            Map(m => m.DutyLocationCounty).Index(HRConstants.DUTY_LOCATION_COUNTY);
            Map(m => m.AgencyCodeSubelement).Index(HRConstants.AGENCY_CODE_SUBELEMENT);
            Map(m => m.SupervisorEmployeeID).Index(HRConstants.SUPERVISOR_EMPLOYEE_ID);
        }
    }

    public sealed class PhoneMap : ClassMap<Phone>
    {
        public PhoneMap()
        {
            Map(m => m.HomePhone).Index(HRConstants.PERSONAL_HOME_PHONE);
            Map(m => m.HomeCell).Index(HRConstants.PERSONAL_CELL_PHONE);
            Map(m => m.WorkPhone).Index(HRConstants.WORK_PHONE_NUMBER);
            Map(m => m.WorkFax).Index(HRConstants.WORK_FAX_NUMBER);
            Map(m => m.WorkCell).Index(HRConstants.WORK_CELL_NUMBER);
            Map(m => m.WorkTextTelephone).Index(HRConstants.WORK_PHONE_NUMBER_TEXT_TELEPHONE);
        }
    }

    public sealed class EmergencyMap : ClassMap<Emergency>
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

    public sealed class InvestigationMap : ClassMap<Investigation>
    {
        public InvestigationMap(List<InvestigationLookup> investigationLookup)
        {
            var investigationConverter = new InvestigationConverter(investigationLookup);

            Map(m => m.PriorInvestigation).Index(HRConstants.PRIOR_INVESTIGATION).TypeConverter(investigationConverter); ;
            Map(m => m.TypeOfInvestigation).Index(HRConstants.INVESTIGATION_TYPE).TypeConverter(investigationConverter); ;
            Map(m => m.DateOfInvestigation).Index(HRConstants.DATE_OF_INVESTIGATION).TypeConverter<DateConverter>();
            Map(m => m.TypeOfInvestigationToRequest).Index(HRConstants.INVESTIGATION_TYPE_REQUESTED).TypeConverter(investigationConverter);
            Map(m => m.InitialResult).Index(HRConstants.INITIAL_RESULT_FINAL_OFFER).TypeConverter<InvistigationResultConverter>();
            Map(m => m.InitialResultDate).Index(HRConstants.INITIAL_RESULT_FINAL_DATE).TypeConverter<DateConverter>();
            Map(m => m.FinalResult).Index(HRConstants.FINAL_RESULT_OFFER).TypeConverter<InvistigationResultConverter>();
            Map(m => m.FinalResultDate).Index(HRConstants.FINAL_RESULT_DATE).TypeConverter<DateConverter>();
            Map(m => m.AdjudicatorEmployeeID).Index(HRConstants.ADJUDICATION_EMPLOYEE_ID);
        }
    }

    //At this time we will not be handling Detail information
    //public sealed class DetailMap : ClassMap<Detail>
    //{
    //    public DetailMap(List<StateLookup> stateLookup)
    //    {
    //        var stateCodeConverter = new StateCodeConverter(stateLookup);

    //        Map(m => m.DetailBeginDate).Index(HRConstants.DETAIL_BEGIN_DATE).TypeConverter<DateConverter>();
    //        Map(m => m.DetialEndDate).Index(HRConstants.DETAIL_END_DATE).TypeConverter<DateConverter>();
    //        Map(m => m.DetailPositionControlNumber).Index(HRConstants.DETAIL_POSITION_CONTROL_NUMBER);
    //        Map(m => m.DetailPositionTitle).Index(HRConstants.DETAIL_POSITION_TITLE);
    //        Map(m => m.DetailOrganizationCode).Index(HRConstants.DETAIL_ORGANIZATION_CODE);
    //        Map(m => m.DetailOfficeSymbol).Index(HRConstants.DETAIL_OFFICE_SYMBOL);
    //        Map(m => m.DetailPayPlan).Index(HRConstants.DETAIL_PAY_PLAN);
    //        Map(m => m.DetailJobSeries).Index(HRConstants.DETAIL_JOB_SERIES);
    //        Map(m => m.DetailLevelGrade).Index(HRConstants.DETAIL_LEVEL_GRADE);
    //        Map(m => m.DetailWorkSchedule).Index(HRConstants.DETAIL_WORK_SCHEDULE);
    //        Map(m => m.DetailRegion).Index(HRConstants.DETAIL_REGION);
    //        Map(m => m.DetailDutyLocationCode).Index(HRConstants.DETAIL_DUTY_LOCATION_CODE);
    //        Map(m => m.DetailDutyLocationCity).Index(HRConstants.DETAIL_DUTY_LOCATION_CITY);
    //        Map(m => m.DetailDutyLocationState).Index(HRConstants.DETAIL_DUTY_LOCATION_STATE).TypeConverter(stateCodeConverter); ;
    //        Map(m => m.DetailDutyLocationCounty).Index(HRConstants.DETAIL_DUTY_LOCATION_COUNTY);
    //    }
    //}
}