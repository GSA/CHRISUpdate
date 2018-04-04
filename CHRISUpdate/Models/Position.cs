using System;

namespace HRUpdate.Models
{
    public class Position
    {
        public string EmployeeID { get; set; }
        public string PositionTitle { get; set; }
        public string PositionOrganization { get; set; }
        //public string PositionNumber { get; set; }
        public string PositionControlNumber { get; set; }
        //public string PositionControlNumberIndicator { get; set; }
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
        //public string DetailPositionControlNumberIndicator { get; set; }
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
    }
}