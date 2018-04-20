﻿using CsvHelper.Configuration;
using HRUpdate.Models;

namespace HRLinks.Mapping
{
    internal sealed class ProcessedSummaryMapping : ClassMap<ProcessedSummary>
    {
        public ProcessedSummaryMapping()
        {
            Map(m => m.GCIMSID).Name("GCIMS ID");
            Map(m => m.FirstName).Name("First Name");
            Map(m => m.MiddleName).Name("Middle Name");
            Map(m => m.LastName).Name("Last Name");
            Map(m => m.Action).Name("Action");
        }
    }

    internal sealed class SeperationMapping : ClassMap<SeperationSummary>
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