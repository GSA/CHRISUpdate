using CsvHelper.Configuration;
using HRUpdate.Models;

namespace HRLinks.Mapping
{
    sealed class SummaryMappng : CsvClassMap<Person>
    {
        public SummaryMappng()
        {
            Map(m => m.PersonID);
            Map(m => m.FirstName);
            Map(m => m.MiddleName);
            Map(m => m.LastName);
            Map(m => m.Action);
        }
    }

    sealed class SeperationMapping: CsvClassMap<SeperationSummary>
    {
        public SeperationMapping()
        {
            Map(m => m.GCIMSID).Name("GCIMS ID");
            Map(m => m.EmployeeID).Name("Employee ID");
            Map(m => m.SeparationCode).Name("Separation Code");
            Map(m => m.Action).Name("Action");
        }

    }
}
