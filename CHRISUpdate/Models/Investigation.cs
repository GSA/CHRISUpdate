using System;

namespace HRUpdate.Models
{
    public class Investigation
    {
        public string PriorInvestigation { get; set; }
        public string TypeOfInvestigation { get; set; }
        public DateTime? DateOfInvestigation { get; set; }
        public string TypeOfInvestigationToRequest { get; set; }
        public string InitialResult { get; set; }
        public DateTime? InitialResultDate { get; set; }
        public string FinalResult { get; set; }
        public string FinalResultDate { get; set; }
        public string AdjudicatorEmployeeID { get; set; }
    }
}
