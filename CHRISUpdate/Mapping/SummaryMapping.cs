﻿using CsvHelper.Configuration;
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
            Map(m => m.Suffix).Name("Suffix");
            Map(m => m.Status).Name("Status");
            Map(m => m.Action).Name("Action");
            Map(m => m.UpdatedColumns).Name("Updated Columns");
        }
    }

    internal sealed class RecordNotFoundSummaryMapping : ClassMap<RecordNotFoundSummary>
    {
        public RecordNotFoundSummaryMapping()
        {
            Map(m => m.GCIMSID).Name("GCIMS ID");
            Map(m => m.EmployeeID).Name("Employee ID");
            Map(m => m.FirstName).Name("First Name");
            Map(m => m.MiddleName).Name("Middle Name");
            Map(m => m.LastName).Name("Last Name");
            Map(m => m.Suffix).Name("Suffix");
        }
    }

    internal sealed class IdenticalRecordSummaryMapping : ClassMap<IdenticalRecordSummary>
    {
        public IdenticalRecordSummaryMapping()
        {
            Map(m => m.GCIMSID).Name("GCIMS ID");
            Map(m => m.EmployeeID).Name("Employee ID");
            Map(m => m.FirstName).Name("First Name");
            Map(m => m.MiddleName).Name("Middle Name");
            Map(m => m.LastName).Name("Last Name");
            Map(m => m.Suffix).Name("Suffix");
            Map(m => m.Status).Name("Status");
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
            Map(m => m.Suffix).Name("Suffix");
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
            Map(m => m.Suffix).Name("Suffix");
            Map(m => m.Status).Name("Status");
        }
    }

    internal sealed class SeperationSummaryMapping : ClassMap<SeparationSummary>
    {
        public SeperationSummaryMapping()
        {
            Map(m => m.GCIMSID).Name("GCIMS ID");
            Map(m => m.EmployeeID).Name("Employee ID");
            Map(m => m.FirstName).Name("First Name");
            Map(m => m.MiddleName).Name("Middle Name");
            Map(m => m.LastName).Name("Last Name");
            Map(m => m.Suffix).Name("Suffix");
            Map(m => m.SeparationCode).Name("Separation Code");
            Map(m => m.SeparationDate).Name("Separation Date");
            Map(m => m.Action).Name("Action");
        }
    }

    internal sealed class SeperationErrorMapping : ClassMap<SeparationSummary>
    {
        public SeperationErrorMapping()
        {
            Map(m => m.GCIMSID).Name("GCIMS ID");
            Map(m => m.EmployeeID).Name("Employee ID");            
            Map(m => m.SeparationCode).Name("Separation Code");
            Map(m => m.SeparationDate).Name("Separation Date");
            Map(m => m.Action).Name("Action");
        }
    }
}