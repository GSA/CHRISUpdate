using System;
using System.Collections.Generic;

namespace HRUpdate.Models
{
    internal class Summary
    {
        public List<ProcessedSummary> ProcessedSummary { get; set; }
        public List<SeparationSummary> SeparationSummary { get; set; }
        public List<RecordNotFoundSummary> RecordNotFoundSummary { get; set; }
        public List<SocialSecurityNumberChangeSummary> SocialSecurityNumberChangeSummary { get; set; }
        public List<InactiveSummary> InactiveSummary { get; set; }
        public EMailData EmailData { get; set; }

        public Summary()
        {
            ProcessedSummary = new List<ProcessedSummary>();
            SeparationSummary = new List<SeparationSummary>();
            RecordNotFoundSummary = new List<RecordNotFoundSummary>();
            SocialSecurityNumberChangeSummary = new List<SocialSecurityNumberChangeSummary>();
            InactiveSummary = new List<InactiveSummary>();
            EmailData = new EMailData();
        }
    }

    internal class ProcessedSummary
    {
        public Int64 GCIMSID { get; set; }
        public string EmployeeID { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Status { get; set; }
        public string Action { get; set; }
    }

    internal class SeparationSummary
    {
        public Int64 GCIMSID { get; set; }
        public string EmployeeID { get; set; }
        public string SeparationCode { get; set; }
        public DateTime? SeparationDate { get; set; }
        public string Action { get; set; }
    }

    internal class RecordNotFoundSummary
    {
        public Int64 GCIMSID { get; set; }
        public string EmployeeID { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Suffix { get; set; }
    }

    internal class SocialSecurityNumberChangeSummary
    {
        public Int64 GCIMSID { get; set; }
        public string EmployeeID { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Status { get; set; }
    }

    internal class InactiveSummary
    {
        public Int64 GCIMSID { get; set; }
        public string EmployeeID { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Status { get; set; }
    }

    internal class EMailData
    {
        public string HRFilename { get; set; }
        public string SEPFileName { get; set; }

        public Int64 HRAttempted { get; set; }
        public Int64 HRSucceeded { get; set; }
        public Int64 HRInactive { get; set; }
        public Int64 HRFailed { get; set; }

        public string HRSuccessfulSummaryFilename { get; set; }
        public string HRErrorSummaryFilename { get; set; }
        public string HRSocialSecurityNumberChangeSummaryFilename { get; set; }
        public string HRInactiveSummaryFilename { get; set; }
        public string HRNameNotFoundFileName { get; set; }

        public bool HRHasErrors { get; set; }

        public Int64 SEPAttempted { get; set; }
        public Int64 SEPSucceeded { get; set; }
        public Int64 SEPFailed { get; set; }

        public string SeparationSuccessfulSummaryFilename { get; set; }
        public string SeparationErrorSummaryFilename { get; set; }

        public bool SEPHasErrors { get; set; }
    }
}