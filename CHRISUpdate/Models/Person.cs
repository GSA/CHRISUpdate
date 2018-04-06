namespace HRUpdate.Models
{
    public class Person
    {        
        public string EmployeeID { get; set; }
        public string LastName { get; set; }
        public string Suffix { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string CordialName { get; set; }
        public string SSN { get; set; }
        public string Gender { get; set; }
        public string SCDLeave { get; set; }
        public string FERO { get; set; }
        public string LEO { get; set; }
        public string Region { get; set; }
        public string OrganizationCode { get; set; }
        public string JobTitle { get; set; }        
        public string HomePhone { get; set; }
        public string HomeCell { get; set; }
        public string HomeEmail { get; set; }
        public string WorkPhone { get; set; }
        public string WorkFax { get; set; }
        public string WorkCell { get; set; }
        public string WorkTTY { get; set; }

        public Address Address { get; set; }
        public Birth Birth { get; set; }
        public Investigation Investigaton { get; set; }
        public Emergency Emergency { get; set; }        
        public Position Position { get; set; }
        public Detail Detail { get; set; }      
    }
}