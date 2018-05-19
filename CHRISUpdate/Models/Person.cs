using System;

namespace HRUpdate.Models
{
    public class Person
    {
        public Int64 GCIMSID { get; set; }
        public string EmployeeID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string Suffix { get; set; }
        public string SocialSecurityNumber { get; set; }
        public string Gender { get; set; }
        public DateTime? ServiceComputationDateLeave { get; set; }
        public bool? FederalEmergencyResponseOfficial { get; set; }
        public bool? LawEnforcementOfficer { get; set; }
        public string Region { get; set; }
        public string OrganizationCode { get; set; }
        public string JobTitle { get; set; }
        public string HomeEmail { get; set; }
        public string Status { get; set; }
    }
}