namespace HRUpdate.Mapping
{
    internal static class HRConstants
    {
        public const int EMPLOYEE_NUMBER = 0;
        public const int EMPLOYEE_LAST_NAME = 1;
        public const int EMPLOYEE_SUFFIX = 2;
        public const int EMPLOYEE_FIRST_NAME = 3;
        public const int EMPLOYEE_MIDDLE_NAME = 4;
        public const int BIRTH_CITY = 5;
        public const int BIRTH_STATE = 6;
        public const int BIRTH_COUNTRY = 7;
        public const int COUNTRY_OF_CITIZENSHIP = 8;
        public const int CITIZEN = 9;
        public const int CORDIAL_NAME = 10;
        public const int AGENCY_CODE = 11;
        public const int HOME_ADDRESS_1 = 12;
        public const int HOME_ADDRESS_2 = 13;
        public const int HOME_CITY = 14;
        public const int HOME_STATE = 15;
        public const int HOME_ZIP_CODE = 16;
        public const int HOME_COUNTRY = 17;
        public const int SOCIAL_SECURITY_NUMBER = 18;
        public const int DATE_OF_BIRTH = 19;
        public const int EMPLOYEE_STATUS = 20;
        public const int GENDER = 21;
        public const int SERVICE_COMPUTATION_DATE_LEAVE = 22;
        public const int PRIOR_INVESTIGATION = 23;
        public const int INVESTIGATION_TYPE = 24;
        public const int DATE_OF_INVESTIGATION = 25;
        public const int INVESTIGATION_TYPE_REQUESTED = 26;
        public const int INITIAL_RESULT_FINAL_OFFER = 27;
        public const int INITIAL_RESULT_FINAL_DATE = 28;
        public const int FINAL_RESULT_OFFER = 29;
        public const int FINAL_RESULT_DATE = 30;
        public const int ADJUDICATION_EMPLOYEE_ID = 31;
        public const int POSITION_CONTROL_NUMBER = 32;        
        public const int SUPERVISORY_STATUS = 34;
        public const int PAY_PLAN = 35;
        public const int JOB_SERIES = 36;
        public const int LEVEL_GRADE = 37;
        public const int WORK_SCHEDULE = 38;        
        public const int LAW_ENFORCEMENT_OFFICER = 40;
        public const int POSITION_TELEWORK_ELIGIBLE = 41;
        public const int POSITION_SENSITIVITY = 42;
        public const int POSITION_START_DATE = 43;
        public const int REGION = 44;
        public const int DUTY_LOCATION_CODE = 45;
        public const int DUTY_LOCATION_CITY = 46;
        public const int DUTY_LOCATION_STATE = 47;
        public const int DUTY_LOCATION_COUNTY = 48;
        public const int AGENCY_CODE_SUBELEMENT = 49;
        public const int SUPERVISOR_EMPLOYEE_ID = 66;
        public const int HOME_ADDRESS_3 = 67;
        /// <summary>
        /// Job Title (This is not truncated like Job Title is in the file)
        /// </summary>
        public const int POSITION_TITLE = 68;
        public const int POSITION_ORGANIZATION = 69;
        public const int PERSONAL_HOME_PHONE = 70;
        public const int PERSONAL_CELL_PHONE = 71;
        public const int PERSONAL_EMAIL_ADDRESS = 72;
        public const int EMERGENCY_POC_NAME = 73;
        public const int EMERGENCY_POC_HOME_PHONE_NUMBER = 74;
        public const int EMERGENCY_POC_WORK_PHONE_NUMBER = 75;
        public const int EMERGENCY_POC_CELL_NUMBER = 76;
        public const int EMERGENCY_OUT_OF_AREA_NAME = 77;
        public const int EMERGENCY_OUT_OF_AREA_HOME_PHONE_NUMBER = 78;
        public const int EMERGENCY_OUT_OF_AREA_HOME_WORK_PHONE_NUMBER = 79;
        public const int EMERGENCY_OUT_OF_AREA_CELL_NUMBER = 80;
        public const int WORK_BUILDING_NUMBER = 81;
        public const int WORK_BUILDING_ADDRESS_LINE_1 = 82;
        public const int WORK_BUILDING_ADDRESS_CITY = 83;
        public const int WORK_BUIDLING_ADDRESS_STATE = 84;
        public const int WORK_BUILDING_ADDRESS_ZIPCODE = 85;
        public const int WORK_PHONE_NUMBER = 86;
        public const int WORK_FAX_NUMBER = 87;
        public const int WORK_CELL_NUMBER = 88;
        public const int WORK_PHONE_NUMBER_TEXT_TELEPHONE = 89;
    }

    internal static class SeparationConstants
    {       
        public const int EMPLOYEE_ID = 0;
        public const int SEPARATION_CODE = 1;
        public const int SEPARATION_DATE = 2;
    }
}