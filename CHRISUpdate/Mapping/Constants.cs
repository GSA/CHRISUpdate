using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

//Create chris name object and use linq to query

namespace HRUpdate.Mapping
{
    public static class HRConstants
    {
        public const int CHRIS_ID = 0; //THIS IS THE EMPLOYEEID

        public const int CHRIS_EMPLOYEE_NUMBER = 0; //UNIQUE EMPLOYEE ID
        //public const int POSITION_NUMBER = 1;

        public const int EMPLOYEE_LAST_NAME = 1;
        public const int EMPLOYEE_SUFFIX = 2;
        //public const int EMPLOYEE_FAMILY_SUFFIX = 2;
        public const int EMPLOYEE_FIRST_NAME = 3;
        public const int EMPLOYEE_MIDDLE_NAME = 4;
        

        //5 pob city
        //6 pob state
        //7 pob country
        //8 country of citizenship
        //9 us citizen
        //10 preferred first name

        public const int AGENCY_CODE = 11;
        public const int HOME_ADDRESS_1 = 12;
        public const int HOME_ADDRESS_2 = 13;
        public const int HOME_CITY = 14;
        public const int HOME_STATE = 15;
        public const int HOME_ZIP_CODE = 16;
        public const int HOME_COUNTRY = 17;
        public const int SSN = 18;

        //19 DOB

        public const int EMPLOYEE_STATUS = 20;
        public const int GENDER = 21;
        public const int SCD_LEAVE = 22;
        public const int PRIOR_COMPLETED = 23;
        public const int TYPE_COMPLETED = 24;
        public const int DATE_COMPLETED = 25;
        public const int TYPE_REQUESTED = 26;

        //27 Initial Result (favorable or not) used to make final offer
        //28 Initial Result (favorable or not) used to make final offer (date)
        //29 Final Result (full investigation results)
        //30 Final Result (full investigation results) date

        //31 Adjudicator's EMPLID

        public const int POSITION_CONTROL_NUMBER = 32;
        public const int JOB_TITLE = 33;
        public const int SUPERVISORY_STATUS = 34;
        public const int PAY_PLAN = 35;
        public const int JOB_SERIES = 36;
        public const int LEVEL_GRADE = 37;
        public const int WORK_SCHEDULE = 38;

        //39 Key Emergency Essential Designation
        //40 LEO Position Indicator

        public const int TELEWORK_ELIGIBLE = 41;
        public const int SENSITIVITY = 42;
        public const int POSITION_START_DATE = 43; //Detail information start date
        public const int REGION = 44;
        public const int DUTY_CODE = 45;
        public const int DUTY_CITY = 46;
        public const int DUTY_STATE = 47;
        public const int DUTY_COUNTY = 48;
        public const int AGENCY_CODE_SUBELEMENT = 49;
        public const int ORGANIZATION_CODE = 50;
        public const int DETAIL_BEGIN_DATE = 51; //Detail Start Date
        public const int DETAIL_END_DATE = 52; //Detail End Date
        public const int DETAIL_POSITION_CONTROL_NUMBER = 53;
        public const int DETAIL_POSITION_TITLE = 54;
        public const int DETAIL_ORGANIZATION_CODE = 55;
        public const int DETAIL_OFFICE_SYMBOL = 56;
        public const int DETAIL_PAY_PLAN = 57;
        public const int DETAIL_JOB_SERIES = 58;
        public const int DETAIL_LEVEL_GRADE = 59;
        public const int DETAIL_WORK_SCHEDULE = 60;
        public const int DETAIL_REGION = 61;
        public const int DETAIL_DUTY_CODE = 62;
        public const int DETAIL_DUTY_CITY = 63;
        public const int DETAIL_DUTY_STATE = 64;
        public const int DETAIL_DUTY_COUNTY = 65;

        public const int SUPERVISOR_CHRIS_ID = 66; //Supervisor Unique ID 

        //public const int SUPERVISOR_EMPLOYEE_NUMBER = ;
        //public const int SUPERVISOR_LAST_NAME_SUFFIX = ;
        //public const int SUPERVISOR_FIRST_NAME = ;
        //public const int SUPERVISOR_MIDDLE_NAME = ;
        //public const int SUPERVISOR_POSITION_CONTROL_NUMBER = ;
        //public const int SUPERVISOR_POSITION_CONTROL_NUMBER_INDICATOR = ;
        //public const int SUPERVISOR_EMAIL = ;

        public const int HOME_ADDRESS_3 = 67;
        public const int POSITION_TITLE = 68;
        public const int POSITION_ORGANIZATION = 69; //Office Symbol

        //70 Personal Home Phone
        //71 Personal Cell Phone
        //72 Personal Email Address
        //73 Emergency Point of Contact Full name
        //74 Emergency POC Home Phone Number
        //75 Emergency POC Work Phone Number
        //76 Emergency POC Cell Number
        //77 Out of Area Emergency Point of Contact Full name
        //78 Out of Area POC Home Phone Number
        //79 Out of Area POC Work Phone Number
        //80 Out of Area POC Cell Number
        //81 Work Building #
        //82 Work Address Line 1
        //83 Work Address City
        //84 Work Address State
        //85 Work Address Zip Code
        //86 Work Phone Number
        //87 Work FAX Number
        //88 Work Cell Number
        //89 Work Phone Number TTY

        //public const int TYPE_OF_EMPLOYMENT = ;
        //public const int HANDICAP = ;
        //public const int ASSIGNMENT_STATUS = ;
        //public const int DUTY_STATUS = ;
        //public const int POSITION_CONTROL_NUMBER_INDICATOR = ;
        public const int ADJUDICATION_AUTH = 31;        
        //public const int DETAIL_POSITION_CONTROL_NUMBER_INDICATOR =;
        

    }

    public class SeparationConstants
    {
        private SeparationConstants() { }

        //public const int UNIQUE_ID = 0;
        public const int CHRIS_ID = 0;
        //public const int LAST_NAME_AND_SUFFIX = 2;
        //public const int FIRST_NAME = 3;
        //public const int MIDDLE_NAME = 4;
        //public const int PREFERRED_NAME = 5;
        //public const int SSN = 6;
        public const int SEPARATION_CODE = 1;
        public const int SEPARATION_DATE = 2;
    }

    public class OrganizationConstants
    {
        private OrganizationConstants() { }

        public const int ABOLISHED_BY_ORDER = 0;
        public const int AGENCY_CODE_SUBLEMENT = 1;
        public const int CHANGED_BY_ORDER = 2;
        public const int CREATED_BY_ORDER = 3;
        public const int DATE_ABOLISHED = 4;
        public const int DATE_OF_LAST_CHANGE = 5;
        public const int FROM_DATE = 6;
        public const int LOCATION = 7;
        public const int LOCATION_ADDRESS = 8;
        public const int NAME = 9;
        public const int OFFICE_SYMBOL = 10;
        public const int OPM_ORGANIZATIONAL_COMPONENT = 11;
        public const int ORGINFO_LINE_1 = 12;
        public const int ORGINFO_LINE_2 = 13;
        public const int ORGINFO_LINE_3 = 14;
        public const int ORGINFO_LINE_4 = 15;
        public const int ORGINFO_LINE_5 = 16;
        public const int ORGINFO_LINE_6 = 17;
        public const int PERSONNEL_OFFICE_IDENTIFIER = 18;
        public const int TO_DATE = 19;
        public const int OCT_ORG_TITLE = 20;
        public const int ORG_CLASSIFICATIONS_NAME = 21;
        public const int ORG_CLASSIFICATIONS_ENABLED = 22;
    }
}