using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace CHRISUpdate.Mapping
{
    public class CHRISConstants
    {
        private CHRISConstants() { }

        public const int ORGANIZATION_CODE = 49;
        public const int CHRIS_EMPLOYEE_NUMBER = 0; //UNIQUE EMPLOYEE ID
        public const int CHRIS_ID = 1; //THIS IS THE EMPLOYEEID
        public const int HOME_ADDRESS_1 = 12;
        public const int HOME_ADDRESS_2 = 13;
        public const int HOME_ADDRESS_3 = 76; 
        public const int HOME_CITY = 14;
        public const int HOME_STATE = 15;
        public const int POSITION_ORGANIZATION = 80;
        public const int HOME_ZIP_CODE = 16;
        public const int HOME_COUNTRY = 17;
        public const int SSN = 18;
        public const int AGENCY_CODE = 11;
        public const int ASSIGNMENT_STATUS = 21;
        public const int DUTY_STATUS = 22;
        public const int EMPLOYEE_STATUS = 20;
        public const int EMPLOYEE_FAMILY_SUFFIX = 2;
        public const int EMPLOYEE_FIRST_NAME = 3;
        public const int GENDER = 23;
        public const int EMPLOYEE_MIDDLE_NAME = 4;
        //publicte const int PERSON_ID = 1;
        public const int SCD_LEAVE = 24;
        public const int SUPERVISORY_STATUS = 33;
        public const int AGENCY_CODE_SUBELEMENT = 48;
        public const int TYPE_OF_EMPLOYMENT = 77;
        public const int HANDICAP = 78;
        public const int DUTY_CITY = 45;
        public const int DUTY_CODE = 44;
        public const int DUTY_COUNTY = 47;
        public const int DUTY_STATE = 46;
        public const int JOB_SERIES = 35;
        public const int JOB_TITLE = 32;
        public const int LEVEL_GRADE = 36;
        public const int OFFICE_SYMBOL = 50;
        public const int PAY_PLAN = 34;
        public const int POSITION_TITLE = 79;
        public const int POSITION_NUMBER = 1;
        public const int POSITION_CONTROL_NUMBER = 30;
        public const int POSITION_CONTROL_NUMBER_INDICATOR = 31;
        public const int REGION = 43;
        public const int SENSITIVITY = 41;
        public const int POSITION_START_DATE = 42; //Detail information start date
        public const int DETAIL_BEGIN_DATE = 51; //Detail Start Date
        public const int DETAIL_END_DATE = 52; //Detail End Date
        public const int DETAIL_POSITION_CONTROL_NUMBER = 53;
        public const int DETAIL_POSITION_CONTROL_NUMBER_INDICATOR = 54;
        public const int DETAIL_POSTION_NUMBER = 55;
        public const int DETAIL_POSITION_TITLE = 56;
        public const int DETAIL_OFFICE_SYMBOL = 58;
        public const int DETAIL_ORGANIZATION_CODE = 57;
        public const int DETAIL_PAY_PLAN = 59;
        public const int DETAIL_JOB_SERIES = 60;
        public const int DETAIL_LEVEL_GRADE = 61;
        public const int DETAIL_WORK_SCHEDULE = 62;
        public const int DETAIL_REGION = 63;
        public const int DETAIL_DUTY_CITY = 65;
        public const int DETAIL_DUTY_CODE = 64;
        public const int DETAIL_DUTY_COUNTY = 67;
        public const int DETAIL_DUTY_STATE = 66;
        public const int TELEWORK_ELIGIBLE = 40;
        public const int WORK_SCHEDULE = 37;
        public const int ADJUDICATION_AUTH = 29;
        public const int DATE_COMPLETED = 27;
        public const int PRIOR_COMPLETED = 25;
        public const int TYPE_COMPLETED = 26;
        public const int TYPE_REQUESTED = 28;
        public const int SUPERVISOR_CHRIS_ID = 69; //Supervisor Unique ID 
        public const int SUPERVISOR_EMAIL = 75;
        public const int SUPERVISOR_FIRST_NAME = 71;
        public const int SUPERVISOR_LAST_NAME_SUFFIX = 70;
        public const int SUPERVISOR_MIDDLE_NAME = 72;
        public const int SUPERVISOR_POSITION_CONTROL_NUMBER = 73;
        public const int SUPERVISOR_POSITION_CONTROL_NUMBER_INDICATOR = 74;
        public const int SUPERVISOR_EMPLOYEE_NUMBER = 68;
    }

    public class SeparationConstants
    {
        private SeparationConstants() { }

        public const int UNIQUE_ID = 0;
        public const int CHRIS_ID = 1;
        public const int LAST_NAME_AND_SUFFIX = 2;
        public const int FIRST_NAME = 3;
        public const int MIDDLE_NAME = 4;
        public const int PREFERRED_NAME = 5;
        public const int SSN = 6;
        public const int SEPARATION_CODE = 7;
        public const int SEPARATION_DATE = 8;
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