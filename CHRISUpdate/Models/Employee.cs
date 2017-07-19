using System;

namespace CHRISUpdate.Models
{
    public class Employee
    {
        public Int64 PersonID { get; set; }
        public string UniqueEmployeeID { get; set; }
        public string EmployeeID { get; set; }
        //This is blank for now.  We are deciding on if we want to store this here and if so we will hash it.                        
        public byte[] SSN { get; set; }
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
    }
}