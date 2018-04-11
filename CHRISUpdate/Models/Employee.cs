using System;

namespace HRUpdate.Models
{
    public class Employee
    {
        public Person Person { get; set; }
        public Address Address { get; set; }
        public Birth Birth { get; set; }
        public Investigation Investigaton { get; set; }
        public Emergency Emergency { get; set; }
        public Position Position { get; set; }
        public Phone Phone { get; set; }
        public Detail Detail { get; set; }
    }
}