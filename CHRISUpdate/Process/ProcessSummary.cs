using HRUpdate.Mapping;
using HRUpdate.Models;
using HRUpdate.Utilities;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace HRUpdate.Process
{
    internal class HRSummary
    {
        //Reference to logger
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public readonly SummaryFileGenerator SummaryFileGenerator;
        public List<InactiveSummary> InactiveRecords { get; set; }
        public List<RecordNotFoundSummary> RecordsNotFound { get; set; }
        public List<IdenticalRecordSummary> IdenticalRecords { get; set; }
        public List<ProcessedSummary> SuccessfulUsersProcessed { get; set; }
        public List<ProcessedSummary> UnsuccessfulUsersProcessed { get; set; }
        public List<SocialSecurityNumberChangeSummary> SocialSecurityNumberChanges { get; set; }

        public HRSummary()
        {
            SummaryFileGenerator = new SummaryFileGenerator();

            InactiveRecords = new List<InactiveSummary>();
            SuccessfulUsersProcessed = new List<ProcessedSummary>();
            IdenticalRecords = new List<IdenticalRecordSummary>();
            UnsuccessfulUsersProcessed = new List<ProcessedSummary>();
            RecordsNotFound = new List<RecordNotFoundSummary>();
            SocialSecurityNumberChanges = new List<SocialSecurityNumberChangeSummary>();
        }

        public void GenerateSummaryFiles(EMailData emailData)
        {
            if (SuccessfulUsersProcessed.Count > 0)
            {
                SuccessfulUsersProcessed = SuccessfulUsersProcessed.OrderBy(o => o.LastName).ThenBy(t => t.FirstName).ToList();

                emailData.HRSuccessfulFilename = SummaryFileGenerator.GenerateSummaryFile<ProcessedSummary, ProcessedSummaryMapping>(ConfigurationManager.AppSettings["SUCCESSSUMMARYFILENAME"].ToString(), SuccessfulUsersProcessed);
                log.Info("HR Success File: " + emailData.HRSuccessfulFilename);
            }

            if (UnsuccessfulUsersProcessed.Count > 0)
            {
                UnsuccessfulUsersProcessed = UnsuccessfulUsersProcessed.OrderBy(o => o.LastName).ThenBy(t => t.FirstName).ToList();

                emailData.HRUnsuccessfulFilename = SummaryFileGenerator.GenerateSummaryFile<ProcessedSummary, ProcessedSummaryMapping>(ConfigurationManager.AppSettings["ERRORSUMMARYFILENAME"].ToString(), UnsuccessfulUsersProcessed);
                log.Info("HR Error File: " + emailData.HRUnsuccessfulFilename);
            }

            if (IdenticalRecords.Count > 0)
            {
                IdenticalRecords = IdenticalRecords.OrderBy(o => o.LastName).ThenBy(t => t.FirstName).ToList();

                emailData.HRIdenticalFilename = SummaryFileGenerator.GenerateSummaryFile<IdenticalRecordSummary, IdenticalRecordSummaryMapping>(ConfigurationManager.AppSettings["IDENTICALSUMMARYFILENAME"].ToString(), IdenticalRecords);
                log.Info("HR Identical File:" + emailData.HRIdenticalFilename);
            }

            if (SocialSecurityNumberChanges.Count > 0)
            {
                SocialSecurityNumberChanges = SocialSecurityNumberChanges.OrderBy(o => o.LastName).ThenBy(t => t.FirstName).ToList();

                emailData.HRSocialSecurityNumberChangeFilename = SummaryFileGenerator.GenerateSummaryFile<SocialSecurityNumberChangeSummary, SocialSecurityNumberChangeSummaryMapping>(ConfigurationManager.AppSettings["SOCIALSECURITYNUMBERCHANGESUMMARYFILENAME"].ToString(), SocialSecurityNumberChanges);
                log.Info("HR Social Security Number Change File: " + emailData.HRSocialSecurityNumberChangeFilename);
            }

            if (InactiveRecords.Count > 0)
            {
                InactiveRecords = InactiveRecords.OrderBy(o => o.LastName).ThenBy(t => t.FirstName).ToList();

                emailData.HRInactiveFilename = SummaryFileGenerator.GenerateSummaryFile<InactiveSummary, InactiveSummaryMapping>(ConfigurationManager.AppSettings["INACTIVESUMMARYFILENAME"].ToString(), InactiveRecords);
                log.Info("HR Inactive File: " + emailData.HRInactiveFilename);
            }

            if (RecordsNotFound.Count > 0)
            {
                RecordsNotFound = RecordsNotFound.OrderBy(o => o.LastName).ThenBy(t => t.FirstName).ToList();

                emailData.HRRecordsNotFoundFileName = SummaryFileGenerator.GenerateSummaryFile<RecordNotFoundSummary, RecordNotFoundSummaryMapping>(ConfigurationManager.AppSettings["RECORDNOTFOUNDSUMMARYFILENAME"].ToString(), RecordsNotFound);
                log.Info("HR Name Not Found File: " + emailData.HRRecordsNotFoundFileName);
            }
        }
    }

    internal class HRSeparationSummary
    {
        //Reference to logger
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public readonly SummaryFileGenerator SummaryFileGenerator;
        public List<SeparationSummary> SuccessfulUsersProcessed { get; set; }
        public List<SeparationSummary> UnsuccessfulUsersProcessed { get; set; }

        public HRSeparationSummary()
        {
            SummaryFileGenerator = new SummaryFileGenerator();

            SuccessfulUsersProcessed = new List<SeparationSummary>();
            UnsuccessfulUsersProcessed = new List<SeparationSummary>();
        }

        public void GenerateSummaryFiles(EMailData emailData)
        {
            if (SuccessfulUsersProcessed.Count > 0)
            {
                SuccessfulUsersProcessed = SuccessfulUsersProcessed.OrderBy(o => o.LastName).ThenBy(t => t.FirstName).ToList();

                emailData.SeparationSuccessfulFilename = SummaryFileGenerator.GenerateSummaryFile<SeparationSummary, SeperationSummaryMapping>(ConfigurationManager.AppSettings["SEPARATIONSUMMARYFILENAME"].ToString(), SuccessfulUsersProcessed);
                log.Info("Separation Success File: " + emailData.SeparationSuccessfulFilename);
            }

            if (UnsuccessfulUsersProcessed.Count > 0)
            {
                UnsuccessfulUsersProcessed = UnsuccessfulUsersProcessed.OrderBy(o => o.EmployeeID).ToList();

                emailData.SeparationErrorFilename = SummaryFileGenerator.GenerateSummaryFile<SeparationSummary, SeperationErrorMapping>(ConfigurationManager.AppSettings["SEPARATIONERRORSUMMARYFILENAME"].ToString(), UnsuccessfulUsersProcessed);
                log.Info("Separation Error File: " + emailData.SeparationErrorFilename);
            }
        }
    }
}