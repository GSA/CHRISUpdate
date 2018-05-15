using CsvHelper.Configuration;
using HRUpdate.Models;

namespace HRUpdate.Mapping
{
    internal sealed class ProcessedSummaryMapping : ClassMap<ProcessedSummary>
    {
        public ProcessedSummaryMapping()
        {
            Map(m => m.GCIMSID).Name("GCIMS ID");
            Map(m => m.EmployeeID).Name("Employee ID");
            Map(m => m.FirstName).Name("First Name");
            Map(m => m.MiddleName).Name("Middle Name");
            Map(m => m.LastName).Name("Last Name");
            Map(m => m.Status).Name("Status");
            Map(m => m.Action).Name("Action");
        }
    }

    internal sealed class SeperationSummaryMapping : ClassMap<SeperationSummary>
    {
        public SeperationSummaryMapping()
        {
            Map(m => m.GCIMSID).Name("GCIMS ID");
            Map(m => m.EmployeeID).Name("Employee ID");
            Map(m => m.SeparationCode).Name("Separation Code");
            Map(m => m.Action).Name("Action");            
        }
    }

    internal sealed class NameNotFoundSummaryMapping : ClassMap<NameNotFoundSummary>
    {
        public NameNotFoundSummaryMapping()
        {
            Map(m => m.GCIMSID).Name("GCIMS ID");
            Map(m => m.EmployeeID).Name("Employee ID");
            Map(m => m.FirstName).Name("First Name");
            Map(m => m.MiddleName).Name("Middle Name");
            Map(m => m.LastName).Name("Last Name");
            Map(m => m.Suffix).Name("Suffix");
            Map(m => m.SSN).Name("SSN");
            Map(m => m.DOB).Name("DOB");
        }
    }

    internal sealed class SocialSecurityNumberChangeSummaryMapping : ClassMap<SocialSecurityNumberChangeSummary>
    {
        public SocialSecurityNumberChangeSummaryMapping()
        {
            Map(m => m.GCIMSID).Name("GCIMS ID");
            Map(m => m.EmployeeID).Name("Employee ID");
            Map(m => m.FirstName).Name("First Name");
            Map(m => m.MiddleName).Name("Middle Name");
            Map(m => m.LastName).Name("Last Name");
            Map(m => m.Status).Name("Status");
        }
    }

    internal sealed class InactiveSummaryMapping : ClassMap<InactiveSummary>
    {
        public InactiveSummaryMapping()
        {
            Map(m => m.GCIMSID).Name("GCIMS ID");
            Map(m => m.EmployeeID).Name("Employee ID");
            Map(m => m.FirstName).Name("First Name");
            Map(m => m.MiddleName).Name("Middle Name");
            Map(m => m.LastName).Name("Last Name");
            Map(m => m.Status).Name("Status");
        }
    }
}