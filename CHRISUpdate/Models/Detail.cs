using System;

namespace HRUpdate.Models
{
    public class Detail
    {
        public DateTime? DetailBeginDate { get; set; }
        public DateTime? DetialEndDate { get; set; }
        public Int64 DetailPositionControlNumber { get; set; }
        public string DetailPositionTitle { get; set; }
        public string DetailOrganizationCode { get; set; }
        public string DetailOfficeSymbol { get; set; }
        public string DetailPayPlan { get; set; }
        public string DetailJobSeries { get; set; }
        public string DetailLevelGrade { get; set; }
        public string DetailWorkSchedule { get; set; }
        public string DetailRegion { get; set; }
        public string DetailDutyLocationCode { get; set; }
        public string DetailDutyLocationCity { get; set; }
        public string DetailDutyLocationState { get; set; }
        public string DetailDutyLocationCounty { get; set; }
    }
}
