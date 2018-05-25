using System;

namespace HRUpdate.Models
{
    internal class ProcessedSummary
    {
        public Int64 GCIMSID { get; set; }
        public string EmployeeID { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Suffix { get; set; }
        public string Status { get; set; }
        public string Action { get; set; }        
    }

    internal class RecordNotFoundSummary
    {
        public Int64 GCIMSID { get; set; }
        public string EmployeeID { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Suffix { get; set; }        
    }

    internal class SocialSecurityNumberChangeSummary
    {
        public Int64 GCIMSID { get; set; }
        public string EmployeeID { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Suffix { get; set; }
        public string Status { get; set; }
    }

    internal class InactiveSummary
    {
        public Int64 GCIMSID { get; set; }
        public string EmployeeID { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Suffix { get; set; }
        public string Status { get; set; }      
    }

    internal class SeparationSummary
    {
        public Int64 GCIMSID { get; set; }
        public string EmployeeID { get; set; }
        public string SeparationCode { get; set; }
        public DateTime? SeparationDate { get; set; }
        public string Action { get; set; }
    }
}