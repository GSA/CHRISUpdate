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
    }
}