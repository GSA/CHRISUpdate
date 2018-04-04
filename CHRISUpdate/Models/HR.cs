namespace HRUpdate.Models
{
    public class HR
    {
        public string EmployeeNumber { get; set; }
        public string ChrisID { get; set; }

        //Readonly (Will always be Government)
        public string Affiliation
        {
            get { return "Government"; }
        }

        public string HomeAddress1 { get; set; }
        public string HomeAddress2 { get; set; }
        public string HomeAddress3 { get; set; }
        public string HomeCity { get; set; }
        public string HomeState { get; set; }
        public string HomeZipCode { get; set; }
        public string HomeCountry { get; set; }
        public string SSN { get; set; }

        //Used for reference mapping
        public Employee Employee { get; set; }
        public Position Position { get; set; }
        public Security Security { get; set; }
        public Supervisor Supervisor { get; set; }
        public Person Person { get; set; }
    }
}