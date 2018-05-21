using HRUpdate.Mapping;
using HRUpdate.Models;
using HRUpdate.Utilities;
using System.Collections.Generic;
using System.Configuration;

namespace HRUpdate.Process
{
    internal class HRSummary
    {
        //Reference to logger
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

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

        public void GenerateSummaryFiles(EMailData emailData)
        {
            if (SuccessfulUsersProcessed.Count > 0)
            {
                emailData.HRSuccessfulFilename = SummaryFileGenerator.GenerateSummaryFile<ProcessedSummary, ProcessedSummaryMapping>(ConfigurationManager.AppSettings["SUCCESSSUMMARYFILENAME"].ToString(), SuccessfulUsersProcessed);
                log.Info("HR Success File: " + emailData.HRSuccessfulFilename);
            }

            if (UnsuccessfulUsersProcessed.Count > 0)
            {
                emailData.HRUnsuccessfulFilename = SummaryFileGenerator.GenerateSummaryFile<ProcessedSummary, ProcessedSummaryMapping>(ConfigurationManager.AppSettings["ERRORSUMMARYFILENAME"].ToString(), UnsuccessfulUsersProcessed);
                log.Info("HR Error File: " + emailData.HRUnsuccessfulFilename);
            }

            if (SocialSecurityNumberChange.Count > 0)
            {
                emailData.HRSocialSecurityNumberChangeFilename = SummaryFileGenerator.GenerateSummaryFile<SocialSecurityNumberChangeSummary, SocialSecurityNumberChangeSummaryMapping>(ConfigurationManager.AppSettings["SOCIALSECURITYNUMBERCHANGESUMMARYFILENAME"].ToString(), SocialSecurityNumberChange);
                log.Info("HR Social Security Number Change File: " + emailData.HRSocialSecurityNumberChangeFilename);
            }

            if (InactiveRecords.Count > 0)
            {
                emailData.HRInactiveFilename = SummaryFileGenerator.GenerateSummaryFile<InactiveSummary, InactiveSummaryMapping>(ConfigurationManager.AppSettings["INACTIVESUMMARYFILENAME"].ToString(), InactiveRecords);
                log.Info("HR Inactive File: " + emailData.HRInactiveFilename);
            }

            if (RecordNotFound.Count > 0)
            {
                emailData.HRRecordsNotFoundFileName = SummaryFileGenerator.GenerateSummaryFile<RecordNotFoundSummary, RecordNotFoundSummaryMapping>(ConfigurationManager.AppSettings["RECORDNOTFOUNDSUMMARYFILENAME"].ToString(), RecordNotFound);
                log.Info("HR Name Not Found File: " + emailData.HRInactiveFilename);
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
                emailData.SeparationSuccessfulFilename = SummaryFileGenerator.GenerateSummaryFile<SeparationSummary, SeperationSummaryMapping>(ConfigurationManager.AppSettings["SEPARATIONSUMMARYFILENAME"].ToString(), SuccessfulUsersProcessed);
                log.Info("Separation Success File: " + emailData.SeparationSuccessfulFilename);
            }

            if (UnsuccessfulUsersProcessed.Count > 0)
            {
                emailData.SeparationErrorFilename = SummaryFileGenerator.GenerateSummaryFile<SeparationSummary, SeperationSummaryMapping>(ConfigurationManager.AppSettings["SEPARATIONERRORSUMMARYFILENAME"].ToString(), UnsuccessfulUsersProcessed);
                log.Info("Separation Error File: " + emailData.SeparationErrorFilename);
            }
        }
    }
}