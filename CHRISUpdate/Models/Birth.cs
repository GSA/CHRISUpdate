using System;

namespace HRUpdate.Models
{
    public class Birth
    {
        public string CityOfBirth { get; set; }
        public string StateOfBirth { get; set; }
        public string CountryOfBirth { get; set; }
        public string CountryOfCitizenship { get; set; }
        public bool? Citizen { get; set; }
        public DateTime? DateOfBirth { get; set; }
    }
}