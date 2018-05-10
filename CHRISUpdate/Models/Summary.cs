using System;

namespace HRUpdate.Models
{
    internal class ProcessedSummary
    {
        public Int64 GCIMSID { get; set; }
        public string EmployeeID { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Action { get; set; }
    }

    internal class SeperationSummary
    {
        public Int64 GCIMSID { get; set; }
        public string EmployeeID { get; set; }
        public string SeparationCode { get; set; }
        public string Action { get; set; }
    }

    internal class NameChangeSummary
    {
        public Int64 GCIMSID { get; set; }
        public string EmployeeID { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
    }

    internal class SocialSecurityNumberChangeSummary
    {
        public Int64 GCIMSID { get; set; }
        public string EmployeeID { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
    }

    internal class InactiveSummary
    {
        public Int64 GCIMSID { get; set; }
        public string EmployeeID { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
    }

    internal class EMailData
    {
        public string HRFilename { get; set; }
        public string SEPFileName { get; set; }

        public Int64 HRAttempted { get; set; }
        public Int64 HRSucceeded { get; set; }
        public Int64 HRFailed { get; set; }

        public string HRSuccessfulSummaryFilename { get; set; }
        public string HRErrorSummaryFilename { get; set; }
        public string HRSocialSecurityNumberChangeSummaryFilename { get; set; }
        public string HRInactiveSummaryFilename { get; set; }

        public bool HRHasErrors { get; set; }

        public Int64 SEPAttempted { get; set; }
        public Int64 SEPSucceeded { get; set; }
        public Int64 SEPFailed { get; set; }

        public string SeparationSuccessfulSummaryFilename { get; set; }
        public string SeparationErrorSummaryFilename { get; set; }

        public bool SEPHasErrors { get; set; }
    }
}