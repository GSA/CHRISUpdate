using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HRUpdate.Models
{
    class Summary
    {
        public Int64 ID { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Action { get; set; }
    }

    class SeperationSummary
    {
        public Int64 GCIMSID { get; set; }
        public Int64 EmployeeID { get; set; }
        public string SeparationCode { get; set; }
        public string Action { get; set; }
    }
}
