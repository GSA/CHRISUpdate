namespace HRUpdate.Models
{
    public class Employee
    {
        public Person Person { get; set; }
        public Address Address { get; set; }
        public Birth Birth { get; set; }
        public Investigation Investigation { get; set; }
        public Emergency Emergency { get; set; }
        public Position Position { get; set; }
        public Phone Phone { get; set; }

        //At this time we will not be handling Detail information
        //public Detail Detail { get; set; }
    }
}