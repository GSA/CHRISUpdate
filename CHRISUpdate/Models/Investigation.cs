using System;

namespace HRUpdate.Models
{
    public class Investigation
    {
        public string PriorInvestigation { get; set; }
        public string TypeOfInvestigation { get; set; }
        public DateTime? DateOfInvestigation { get; set; }
        public string TypeOfInvestigationToRequest { get; set; }
        public bool InitialResult { get; set; }
        public DateTime? InitialResultDate { get; set; }
        public bool FinalResult { get; set; }
        public DateTime? FinalResultDate { get; set; }
        public string AdjudicatorEmployeeID { get; set; }
    }
}