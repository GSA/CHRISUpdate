using CsvHelper;
using CsvHelper.Configuration;
using HRLinks.Mapping;
using HRUpdate.Mapping;
using HRUpdate.Models;
using HRUpdate.Utilities;
using KellermanSoftware.CompareNetObjects;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;

namespace HRUpdate.Process
{
    internal class ProcessData
    {
        //Reference to logger
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly SummaryFileGenerator summaryFileGenerator = new SummaryFileGenerator();
        private readonly SaveData save = new SaveData();

        private readonly EMailData emailData = new EMailData();
        private readonly Helpers helper = new Utilities.Helpers();

        //Constructor
        public ProcessData() { }        
        
        private bool AreEqualGCIMSToHR(Employee GCIMSData, Employee HRData)
        {
            CompareLogic compareLogic = new CompareLogic();
            compareLogic.Config.MembersToIgnore.Add("Person.SocialSecurityNumber");
            compareLogic.Config.MembersToIgnore.Add("Detail");            

            ComparisonResult result = compareLogic.Compare(GCIMSData, HRData);

            return result.AreEqual;            
        }

        /// <summary>
        /// Get HR Data
        /// Loop HR Data
        /// Get GCIMS Record
        /// Update GCIMS Record
        /// </summary>
        /// <param name="hrFile"></param>
        public void ProcessHRFile(string HRFile)
        {
            log.Info("Processing HR Users");

            try
            {
                List<Employee> usersToProcess;
                List<ProcessedSummary> successfulHRUsersProcessed = new List<ProcessedSummary>();
                List<ProcessedSummary> unsuccessfulHRUsersProcessed = new List<ProcessedSummary>();
                
                usersToProcess = GetFileData<Employee, EmployeeMapping>(HRFile);

                Tuple<int, int, string, string, Employee> personResults;
                Tuple<int, string, string> updatedResults;

                //Start Processing the HR Data
                foreach (Employee employeeData in usersToProcess)
                {
                    personResults = save.GetGCIMSRecord(employeeData.Person.EmployeeID, helper.HashSSN(employeeData.Person.SocialSecurityNumber), employeeData.Person.LastName, employeeData.Birth.DateOfBirth?.ToString("yyyy-M-dd"));

                    int personID = personResults.Item1;                    
                    
                    if (personID > 0 && !AreEqualGCIMSToHR(personResults.Item5, employeeData))
                    {
                        log.Info("Updating Record:" + personResults.Item1);                        

                        updatedResults = save.UpdatePersonInformation(personResults.Item1, employeeData);

                        if (updatedResults.Item1 > 0)
                        {
                            var processedUserSuccess = usersToProcess
                             .Where(w => w.Person.EmployeeID == employeeData.Person.EmployeeID)
                             .Select
                                 (
                                     s =>
                                         new ProcessedSummary
                                         {
                                             GCIMSID = personResults.Item1,
                                             FirstName = s.Person.FirstName,
                                             MiddleName = s.Person.MiddleName,
                                             LastName = s.Person.LastName,
                                             Action = updatedResults.Item2
                                         }
                                 ).ToList();

                            successfulHRUsersProcessed.AddRange(processedUserSuccess);

                            log.Info("Successfully Updated Record: " + personResults.Item1);
                        }
                        else
                        {
                            var proccessedUserIssue = usersToProcess
                            .Where(w => w.Person.EmployeeID == employeeData.Person.EmployeeID)
                            .Select
                                 (
                                     s =>
                                         new ProcessedSummary
                                         {
                                             GCIMSID = personResults.Item1,
                                             FirstName = s.Person.FirstName,
                                             MiddleName = s.Person.MiddleName,
                                             LastName = s.Person.LastName,
                                             Action = updatedResults.Item3
                                         }
                                 ).ToList();

                            unsuccessfulHRUsersProcessed.AddRange(proccessedUserIssue);
                        }
                    }
                    else
                    {
                        var proccessedUserIssue = usersToProcess
                            .Where(w => w.Person.EmployeeID == employeeData.Person.EmployeeID)
                            .Select
                                 (
                                     s =>
                                         new ProcessedSummary
                                         {
                                             GCIMSID = personResults.Item1,
                                             FirstName = s.Person.FirstName,
                                             MiddleName = s.Person.MiddleName,
                                             LastName = s.Person.LastName,
                                             Action = personResults.Item3
                                         }
                                 ).ToList();

                        unsuccessfulHRUsersProcessed.AddRange(proccessedUserIssue);
                    }
                }

                emailData.HRFilename = Path.GetFileName(HRFile);
                emailData.HRAttempted = usersToProcess.Count;
                emailData.HRSucceeded = successfulHRUsersProcessed.Count;
                emailData.HRFailed = unsuccessfulHRUsersProcessed.Count;
                emailData.HRHasErrors = unsuccessfulHRUsersProcessed.Count > 0 ? true : false;

                //Add log entries
                log.Info("HR Records Updated: " + String.Format("{0:#,###0}", successfulHRUsersProcessed.Count));
                log.Info("HR Users Not Processed: " + String.Format("{0:#,###0}", unsuccessfulHRUsersProcessed.Count));
                log.Info("HR Records Processed: " + String.Format("{0:#,###0}", usersToProcess.Count));

                GenerateUsersProccessedSummaryFiles(successfulHRUsersProcessed, unsuccessfulHRUsersProcessed);
            }
            //Catch all errors
            catch (Exception ex)
            {
                log.Error("Process HR Users Error:" + ex.Message + " " + ex.InnerException + " " + ex.StackTrace);                
            }
        }

        /// <summary>
        /// Process separation file
        /// </summary>
        /// <param name="separationFile"></param>
        public void ProcessSeparationFile(string SEPFile)
        {
            log.Info("Processing Separation Users");
                        
            try
            {
                List<Separation> separationUsersToProcess;
                List<SeperationSummary> successfulSeparationUsersProcessed = new List<SeperationSummary>();
                List<SeperationSummary> unsuccessfulSeparationUsersProcessed = new List<SeperationSummary>();

                separationUsersToProcess = GetFileData<Separation, CustomSeparationMap>(SEPFile);

                Tuple<int, int, string, string> separationResults;

                foreach (Separation separationData in separationUsersToProcess)
                {
                    separationResults = save.SaveSeparationInformation(separationData);

                    if (separationResults.Item1 > 0)
                    {
                        log.Info("Separating User: " + separationResults.Item1);

                        var separationSuccess = separationUsersToProcess
                             .Where(w => w.EmployeeID == separationData.EmployeeID)
                             .Select
                                 (
                                     s =>
                                         new SeperationSummary
                                         {
                                             GCIMSID = separationResults.Item1,
                                             EmployeeID = s.EmployeeID,
                                             SeparationCode = s.SeparationCode,
                                             Action = separationResults.Item3
                                         }
                                 ).ToList();

                        successfulSeparationUsersProcessed.AddRange(separationSuccess);

                        log.Info("Successfully Separated Record: " + separationResults.Item1);
                    }
                    else
                    {
                        var separationIssue = separationUsersToProcess
                            .Where(w => w.EmployeeID == separationData.EmployeeID)
                            .Select
                                (
                                    s =>
                                        new SeperationSummary
                                        {
                                            GCIMSID = separationResults.Item1,
                                            EmployeeID = s.EmployeeID,
                                            SeparationCode = s.SeparationCode,
                                            Action = separationResults.Item3
                                        }
                                ).ToList();

                        unsuccessfulSeparationUsersProcessed.AddRange(separationIssue);
                    }             
                }

                emailData.SEPFileName = Path.GetFileName(SEPFile);
                emailData.SEPAttempted = separationUsersToProcess.Count;
                emailData.SEPSucceeded = successfulSeparationUsersProcessed.Count;
                emailData.SEPFailed = unsuccessfulSeparationUsersProcessed.Count;
                emailData.SEPHasErrors = unsuccessfulSeparationUsersProcessed.Count > 0 ? true : false;

                log.Info("Separation Records Processed: " + String.Format("{0:#,###0}", successfulSeparationUsersProcessed.Count));
                log.Info("Separation Users Not Processed: " + String.Format("{0:#,###0}", unsuccessfulSeparationUsersProcessed.Count));
                log.Info("Separation Processed Records: " + String.Format("{0:#,###0}", separationUsersToProcess.Count));

                GenerateSeparationSummaryFiles(successfulSeparationUsersProcessed, unsuccessfulSeparationUsersProcessed);
            }
            catch (Exception ex)
            {   
                log.Error("Process Separation Users Error:" + ex.Message + " " + ex.InnerException); 
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="processedSuccessSummary"></param>
        /// <param name="processedErrorSummary"></param>
        private void GenerateUsersProccessedSummaryFiles(List<ProcessedSummary> usersProcessedSuccessSummary, List<ProcessedSummary> usersProcessedErrorSummary)
        {
            if (usersProcessedSuccessSummary.Count > 0)
            {
                emailData.HRSuccessfulSummaryFilename = summaryFileGenerator.GenerateSummaryFile<ProcessedSummary, ProcessedSummaryMapping>(ConfigurationManager.AppSettings["SUCCESSSUMMARYFILENAME"].ToString(), usersProcessedSuccessSummary);
                log.Info("HR Success File: " + emailData.HRSuccessfulSummaryFilename);
            }

            if (usersProcessedErrorSummary.Count > 0)
            {
                emailData.HRErrorSummaryFilename = summaryFileGenerator.GenerateSummaryFile<ProcessedSummary, ProcessedSummaryMapping>(ConfigurationManager.AppSettings["ERRORSUMMARYFILENAME"].ToString(), usersProcessedErrorSummary);
                log.Info("HR Error File: " + emailData.HRErrorSummaryFilename);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="separationSuccessSummary"></param>
        /// <param name="separationErrorSummary"></param>
        private void GenerateSeparationSummaryFiles(List<SeperationSummary> separationSuccessSummary, List<SeperationSummary> separationErrorSummary)
        {
            if (separationSuccessSummary.Count > 0)
            {
                emailData.SeparationSuccessfulSummaryFilename = summaryFileGenerator.GenerateSummaryFile<SeperationSummary, SeperationMapping>(ConfigurationManager.AppSettings["SEPARATIONSUMMARYFILENAME"].ToString(), separationSuccessSummary);
                log.Info("Separation Success File: " + emailData.SeparationSuccessfulSummaryFilename);
            }

            if (separationErrorSummary.Count > 0)
            {
                emailData.SeparationErrorSummaryFilename = summaryFileGenerator.GenerateSummaryFile<SeperationSummary, SeperationMapping>(ConfigurationManager.AppSettings["SEPARATIONERRORSUMMARYFILENAME"].ToString(), separationErrorSummary);
                log.Info("Separation Error File: " + emailData.SeparationErrorSummaryFilename);
            }
        }

        internal void SendSummaryEMail()
        {
            EMail email = new EMail();
            
            string subject = string.Empty;
            string body = string.Empty;
            string attahcments = string.Empty;         

            subject = ConfigurationManager.AppSettings["EMAILSUBJECT"].ToString() + " - " + DateTime.Now.ToString("MMMM dd, yyyy HH:mm:ss");

            body = GenerateEMailBody();

            attahcments = SummaryAttachments();

            using (email)
            {
                email.Send(ConfigurationManager.AppSettings["DEFAULTEMAIL"].ToString(),
                           ConfigurationManager.AppSettings["TO"].ToString(),
                           ConfigurationManager.AppSettings["CC"].ToString(),
                           ConfigurationManager.AppSettings["BCC"].ToString(),
                           subject, body, attahcments.TrimEnd(';'), ConfigurationManager.AppSettings["SMTPSERVER"].ToString(), true);
            }
        }

        public string GenerateEMailBody()
        {
            StringBuilder errors = new StringBuilder();
            StringBuilder fileNames = new StringBuilder();

            string template = File.ReadAllText(ConfigurationManager.AppSettings["SUMMARYTEMPLATE"]);
            
            fileNames.Append(emailData.HRFilename == null ? "No HR Links File Found" : emailData.HRFilename.ToString());
            fileNames.Append(", ");
            fileNames.Append(emailData.SEPFileName == null ? "No Separation File Found" : emailData.SEPFileName.ToString());
            
            template = template.Replace("[FILENAMES]", fileNames.ToString());

            template = template.Replace("[HRATTEMPTED]", emailData.HRAttempted.ToString());
            template = template.Replace("[HRSUCCEEDED]", emailData.HRSucceeded.ToString());
            template = template.Replace("[HRFAILED]", emailData.HRFailed.ToString());

            if (emailData.HRHasErrors)
            {
                errors.Clear();

                errors.Append("<b><font color='red'>Errors were found while processing the HR file</font></b><br />");
                errors.Append("<br />Please see the attached file: <b><font color='red'>");
                errors.Append(emailData.HRErrorSummaryFilename);
                errors.Append("</font></b>");

                template = template.Replace("[IFHRERRORS]", errors.ToString());
            }
            else
            {
                template = template.Replace("[IFHRERRORS]", null);
            }

            template = template.Replace("[SEPATTEMPTED]", emailData.SEPAttempted.ToString());
            template = template.Replace("[SEPSUCCEEDED]", emailData.SEPSucceeded.ToString());
            template = template.Replace("[SEPFAILED]", emailData.SEPFailed.ToString());

            if (emailData.SEPHasErrors)
            {
                errors.Clear();

                errors.Append("<b><font color='red'>Errors were found while processing the separation file</font></b><br />");
                errors.Append("<br />Please see the attached file: <b><font color='red'>");
                errors.Append(emailData.SeparationErrorSummaryFilename);
                errors.Append("</font></b>");

                template = template.Replace("[IFSEPERRORS]", errors.ToString());
            }
            else
            {
                template = template.Replace("[IFSEPERRORS]", null);
            }

            return template;
        }

        private string SummaryAttachments()
        {
            StringBuilder attachments = new StringBuilder();

            if (emailData.HRSuccessfulSummaryFilename != null)
                attachments.Append(AddAttachment(emailData.HRSuccessfulSummaryFilename));

            if (emailData.HRErrorSummaryFilename != null)
                attachments.Append(AddAttachment(emailData.HRErrorSummaryFilename));

            if (emailData.SeparationSuccessfulSummaryFilename != null)
                attachments.Append(AddAttachment(emailData.SeparationSuccessfulSummaryFilename));

            if (emailData.SeparationErrorSummaryFilename != null)
                attachments.Append(AddAttachment(emailData.SeparationErrorSummaryFilename));

            return attachments.ToString();
        }

        private string AddAttachment(string fileName)
        {
            StringBuilder addAttachment = new StringBuilder();

            addAttachment.Append(ConfigurationManager.AppSettings["SUMMARYFILEPATH"]);
            addAttachment.Append(fileName);
            addAttachment.Append(";");

            return addAttachment.ToString();
        }

        /// <summary>
        /// Takes a file and loads the data into the object type specified using the mapping
        /// </summary>
        /// <typeparam name="TClass"></typeparam>
        /// <typeparam name="TMap"></typeparam>
        /// <param name="filePath"></param>
        /// <param name="config"></param>
        /// <returns>A list of the type of objects specified</returns>
        private List<TClass> GetFileData<TClass, TMap>(string filePath)
            where TClass : class
            where TMap : ClassMap<TClass>
        {
            CsvParser csvParser = new CsvParser(new StreamReader(filePath));
            CsvReader csvReader = new CsvReader(csvParser);

            csvReader.Configuration.Delimiter = "~";
            csvReader.Configuration.HasHeaderRecord = false;

            csvReader.Configuration.RegisterClassMap<TMap>();

            return csvReader.GetRecords<TClass>().ToList();
        }
    }
}