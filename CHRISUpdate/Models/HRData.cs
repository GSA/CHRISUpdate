using System;

namespace HRUpdate.Models
{
    class HRData
    {
        public string EmployeeNumber { get; set; }
        public string ChrisID { get; set; }

        //Readonly (Will always be Government)
        public string Affiliation
        {
            get { return "Government"; }
        }

        public string HomeAddress1 { get; set; }
        public string HomeAddress2 { get; set; }
        public string HomeAddress3 { get; set; }
        public string HomeCity { get; set; }
        public string HomeState { get; set; }
        public string HomeZipCode { get; set; }
        public string HomeCountry { get; set; }
        public string SSN { get; set; }

        #region Employee
            public Int64 PersonID { get; set; }
            public string UniqueEmployeeID { get; set; }
            public string EmployeeID { get; set; }
            //This is blank for now.  We are deciding on if we want to store this here and if so we will hash it.
            public byte[] EmployeeSSN { get; set; }
            public string AgencyCode { get; set; }
            public string EmployeeStatus { get; set; }
            public string TypeOfemployment { get; set; }
            public string Handicap { get; set; }
            public string DutyStatus { get; set; }
            public string AssignmentStatus { get; set; }
            public DateTime? SCDLeave { get; set; }
            public string FamilySuffix { get; set; }
            public string FirstName { get; set; }
            public string MiddleName { get; set; }
            public string Gender { get; set; }
            public string SupervisoryStatus { get; set; }
        #endregion

        #region Position
            public string PositionEmployeeID { get; set; }
            public string PositionTitle { get; set; }
            public string PositionOrganization { get; set; }
            public string PositionNumber { get; set; }
            public string PositionControlNumber { get; set; }
            public string PositionControlNumberIndicator { get; set; }
            public string AgencyCodeSubelment { get; set; }
            public string TeleworkEligible { get; set; }
            public string Sensitivity { get; set; }

            //Need to fix this without having two?
            public DateTime? StartDate { get; set; }
            public DateTime? EndDate { get; set; }
            public string JobTitle { get; set; }
            public string OrganizationCode { get; set; }
            public string OfficeSymbol { get; set; }
            public string PayPlan { get; set; }
            public string JobSeries { get; set; }
            public string LevelGrade { get; set; }
            public string WorkSchedule { get; set; }
            public string Region { get; set; }
            public string DutyCode { get; set; }
            public string DutyCity { get; set; }
            public string DutyState { get; set; }
            public string DutyCounty { get; set; }
            public bool IsDetail { get; set; }

            //Detail Information
            public DateTime? DetailBeginDate { get; set; }
            public DateTime? DetailEndDate { get; set; }
            public string DetailPositionControlNumber { get; set; }
            public string DetailPositionControlNumberIndicator { get; set; }
            public string DetailPositionNumber { get; set; }

            //Same as JobTitle
            public string DetailPositionTitle { get; set; }
            public string DetailOfficeSymbol { get; set; }
            public string DetailOrganizationCode { get; set; }
            public string DetailPayPlan { get; set; }
            public string DetailJobSeries { get; set; }
            public string DetailLevelGrade { get; set; }
            public string DetailWorkSchedule { get; set; }
            public string DetailRegion { get; set; }
            public string DetailDutyCity { get; set; }
            public string DetailDutyCode { get; set; }
            public string DetailDutyCounty { get; set; }
            public string DetailDutyState { get; set; }
        #endregion

        #region Security
            public string SecurityEmployeeID { get; set; }
            public string TypeCompleted { get; set; }
            public DateTime? DateCompleted { get; set; }
            public string PriorCompleted { get; set; }
            public string TypeRequested { get; set; }
            public string AdjudicationAuth { get; set; }
        #endregion

        #region Supervisor
            public string SupervisorChrisEmployeeID { get; set; }
            public string SupervisorUniqueEmployeeID { get; set; }
            public string SupervisorEmployeeID { get; set; }
            public string SupervisorLastNameSuffix { get; set; }
            public string SupervisorFirstName { get; set; }
            public string SupervisorMiddleName { get; set; }
            public string SupervisorPositionControlNumber { get; set; }
            public string SupervisorPositionControlNumberIndicator { get; set; }
            public string EMail { get; set; }
        #endregion

    }
}
