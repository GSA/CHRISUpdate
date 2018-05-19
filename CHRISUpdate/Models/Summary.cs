using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;

namespace HRUpdate.Models
{
    internal class HRSummary
    {
        public readonly SummaryFileGenerator SummaryFileGenerator;
        public List<InactiveSummary> InactiveRecords { get; set; }
        public List<RecordNotFoundSummary> RecordNotFound { get; set; }
        public List<ProcessedSummary> SuccessfulUsersProcessed { get; set; }
        public List<ProcessedSummary> UnsuccessfulUsersProcessed { get; set; }
        public List<SocialSecurityNumberChangeSummary> SocialSecurityNumberChange { get; set; }

        public HRSummary()
        {
            SummaryFileGenerator = new SummaryFileGenerator();

            InactiveRecords = new List<InactiveSummary>();
            SuccessfulUsersProcessed = new List<ProcessedSummary>();
            UnsuccessfulUsersProcessed = new List<ProcessedSummary>();
            RecordNotFound = new List<RecordNotFoundSummary>();
            SocialSecurityNumberChange = new List<SocialSecurityNumberChangeSummary>();
        }
    }

    internal class HRSeparationSummary
    {
        public readonly SummaryFileGenerator SummaryFileGenerator;
        public List<SeparationSummary> SuccessfulUsersProcessed { get; set; }
        public List<SeparationSummary> UnsuccessfulUsersProcessed { get; set; }

        public HRSeparationSummary()
        {
            SummaryFileGenerator = new SummaryFileGenerator();

            SuccessfulUsersProcessed = new List<SeparationSummary>();
            UnsuccessfulUsersProcessed = new List<SeparationSummary>();
        }
    }

    internal class ProcessedSummary
    {
        public Int64 GCIMSID { get; set; }
        public string EmployeeID { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Suffix { get; set; }
        public string Status { get; set; }
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
        public string Suffix { get; set; }
        public string Status { get; set; }
    }

    internal class InactiveSummary
    {
        public Int64 GCIMSID { get; set; }
        public string EmployeeID { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Suffix { get; set; }
        public string Status { get; set; }
    }

    internal class SeparationSummary
    {
        public Int64 GCIMSID { get; set; }
        public string EmployeeID { get; set; }
        public string SeparationCode { get; set; }
        public DateTime? SeparationDate { get; set; }
        public string Action { get; set; }
    }

    internal class SummaryFileGenerator
    {
        //Reference to logger
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //Empty Constructor
        public SummaryFileGenerator() { }

        internal string GenerateSummaryFile<TClass, TMap>(string fileName, List<TClass> summaryData)
            where TClass : class
            where TMap : ClassMap<TClass>
        {
            try
            {
                string summaryFileName;

                summaryFileName = fileName + "_" + DateTime.Now.ToString("yyyyMMddHHmmss_FFFF") + ".csv";

                //Creates the summary file
                using (CsvWriter csvWriter = new CsvWriter(new StreamWriter(ConfigurationManager.AppSettings["SUMMARYFILEPATH"] + summaryFileName, false)))
                {
                    csvWriter.Configuration.RegisterClassMap<TMap>();
                    csvWriter.WriteRecords(summaryData);
                }

                return summaryFileName;
            }
            catch (Exception ex)
            {
                log.Error("Error Writing Summary File: " + fileName + " - " + ex.Message + " - " + ex.InnerException);
                return string.Empty;
            }
        }
    }

    internal class EMailData
    {
        public string HRFilename { get; set; }
        public string SEPFileName { get; set; }

        public Int64 HRFailed { get; set; }
        public Int64 HRSocial { get; set; }
        public Int64 HRInactive { get; set; }
        public Int64 HRAttempted { get; set; }
        public Int64 HRSucceeded { get; set; }
        public Int64 HRRecordsNotFound { get; set; }

        public string HRSuccessfulFilename { get; set; }
        public string HRUnsuccessfulFilename { get; set; }
        public string HRSocialSecurityNumberChangeFilename { get; set; }
        public string HRInactiveFilename { get; set; }
        public string HRRecordsNotFoundFileName { get; set; }

        public bool HRHasErrors { get; set; }

        public Int64 SEPAttempted { get; set; }
        public Int64 SEPSucceeded { get; set; }
        public Int64 SEPFailed { get; set; }

        public string SeparationSuccessfulFilename { get; set; }
        public string SeparationErrorFilename { get; set; }

        public bool SEPHasErrors { get; set; }
    }
}