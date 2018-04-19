using System;

namespace HRUpdate.Models
{
    public class Detail
    {
        //private string _DetailRegion { get; set; }
        //private bool _IsDetail { get; set; }

        public DateTime? DetailBeginDate { get; set; }
        public DateTime? DetialEndDate { get; set; }
        public string DetailPositionControlNumber { get; set; }
        public string DetailPositionTitle { get; set; }
        public string DetailOrganizationCode { get; set; }
        public string DetailOfficeSymbol { get; set; }
        public string DetailPayPlan { get; set; }
        public string DetailJobSeries { get; set; }
        public string DetailLevelGrade { get; set; }
        public string DetailWorkSchedule { get; set; }
        public string DetailRegion { get; set; }
        //public string DetailRegion
        //{
        //    get { return _DetailRegion; }
        //    set 
        //    {
        //        _DetailRegion = value;
        //        IsDetail = DetermineDetailee(value, _DetailRegion);                                   
        //    }
        //}
        public string DetailDutyLocationCode { get; set; }
        public string DetailDutyLocationCity { get; set; }
        public string DetailDutyLocationState { get; set; }
        public string DetailDutyLocationCounty { get; set; }
        //public bool IsDetail { get; set; }

        //private bool DetermineDetailee(string value, string backingField)
        //{
        //    backingField = value;

        //    return value.Length > 0;
        //}
    }
}