using System;

namespace CHRISUpdate.Models
{
    public class Security
    {
        public string EmployeeID { get; set; }
        public string TypeCompleted { get; set; }
        public DateTime? DateCompleted { get; set; }
        public string PriorCompleted { get; set; }
        public string TypeRequested { get; set; }
        public string AdjudicationAuth { get; set; }
    }
}
