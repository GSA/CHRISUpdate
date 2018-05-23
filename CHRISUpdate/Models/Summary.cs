using System;

namespace HRUpdate.Models
{
    internal class ProcessedSummary : IComparable<ProcessedSummary>
    {
        public Int64 GCIMSID { get; set; }
        public string EmployeeID { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Suffix { get; set; }
        public string Status { get; set; }
        public string Action { get; set; }

        public int CompareTo(ProcessedSummary other)
        {
            var compare = this.LastName.ToLower().CompareTo(other.LastName.ToLower());

            if (compare == 1)
                compare = this.FirstName.ToLower().CompareTo(other.FirstName.ToLower());

            return compare;
        }
    }

    internal class RecordNotFoundSummary : IComparable<RecordNotFoundSummary>
    {
        public Int64 GCIMSID { get; set; }
        public string EmployeeID { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Suffix { get; set; }

        public int CompareTo(RecordNotFoundSummary other)
        {
            var compare = this.LastName.ToLower().CompareTo(other.LastName.ToLower());

            if (compare == 1)
                compare = this.FirstName.ToLower().CompareTo(other.FirstName.ToLower());

            return compare;
        }
    }

    internal class SocialSecurityNumberChangeSummary : IComparable<SocialSecurityNumberChangeSummary>
    {
        public Int64 GCIMSID { get; set; }
        public string EmployeeID { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Suffix { get; set; }
        public string Status { get; set; }

        public int CompareTo(SocialSecurityNumberChangeSummary other)
        {
            var compare = this.LastName.ToLower().CompareTo(other.LastName.ToLower());

            if (compare == 1)
                compare = this.FirstName.ToLower().CompareTo(other.FirstName.ToLower());

            return compare;
        }
    }

    internal class InactiveSummary : IComparable<InactiveSummary>
    {
        public Int64 GCIMSID { get; set; }
        public string EmployeeID { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Suffix { get; set; }
        public string Status { get; set; }
        
        public int CompareTo(InactiveSummary other)
        {
            var compare = this.LastName.ToLower().CompareTo(other.LastName.ToLower());

            if (compare == 1)
                compare = this.FirstName.ToLower().CompareTo(other.FirstName.ToLower());

            return compare;
        }
    }

    internal class SeparationSummary : IComparable<SeparationSummary>
    {
        public Int64 GCIMSID { get; set; }
        public string EmployeeID { get; set; }
        public string SeparationCode { get; set; }
        public DateTime? SeparationDate { get; set; }
        public string Action { get; set; }

        public int CompareTo(SeparationSummary other)
        {
            return this.EmployeeID.CompareTo(other.EmployeeID);
        }
    }
}