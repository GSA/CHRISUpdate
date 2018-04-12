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

        //Constructor
        public ProcessData() { }        

        public void CompareObject(string chrisFile)
        {            
            List<Employee> hrData;
            List<Employee> gcimsData;           

            CompareLogic compareLogic = new CompareLogic();
            compareLogic.Config.MaxDifferences = 500;

            hrData = GetFileData<Employee, EmployeeMapping>(chrisFile);
            gcimsData = GetFileData<Employee, EmployeeMapping>(chrisFile);

            var hrFilter = hrData.Where(w => w.Person.EmployeeID == "00004624");
            var gcimsFilter = gcimsData.Where(w => w.Person.EmployeeID == "00007172");            

            ComparisonResult result = compareLogic.Compare(hrFilter, gcimsFilter);

            if (!result.AreEqual)
                Console.WriteLine(result.DifferencesString);

            Console.ReadLine();
        }

        private bool AreEqualGCIMSToHR(Employee GCIMSData, Employee HRData)
        {
            CompareLogic compareLogic = new CompareLogic();
            compareLogic.Config.MembersToIgnore.Add("Person.SSN");

            ComparisonResult result = compareLogic.Compare(GCIMSData, HRData);

            return result.AreEqual;

            //if (!result.AreEqual)
            //    Console.WriteLine(result.DifferencesString);
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

                //Successful Users Processed
                List<ProcessedSummary> successfulHRUsersProcessed = new List<ProcessedSummary>();

                //Unsuccessful Users Processed
                List<ProcessedSummary> unsuccessfulHRUsersProcessed = new List<ProcessedSummary>();

                Utilities.Helpers helper = new Utilities.Helpers();

                //Call function to map file to csv
                usersToProcess = GetFileData<Employee, EmployeeMapping>(HRFile);

                Tuple<int, int, string, string, Employee> personResults;

                //Start Processin the HR Data
                foreach (Employee employeeData in usersToProcess)
                {
                    personResults = save.GetGCIMSRecord(employeeData.Person.EmployeeID, helper.HashSSN(employeeData.Person.SSN), employeeData.Person.LastName, employeeData.Birth.DateOfBirth?.ToString("yyyy-M-dd"));
                    
                    int personID = personResults.Item1;                    
                    
                    if (personID > 0 && !AreEqualGCIMSToHR(personResults.Item5, employeeData))
                    {
                        Console.WriteLine("Update Record");

                        //SaveData

                        var processedUserSuccess = usersToProcess
                             .Where(w => w.Person.EmployeeID == employeeData.Person.EmployeeID)
                             .Select
                                 (
                                     s =>
                                         new ProcessedSummary
                                         {
                                             ID = personResults.Item1,
                                             FirstName = s.Person.FirstName,
                                             MiddleName = s.Person.MiddleName,
                                             LastName = s.Person.LastName,
                                             Action = personResults.Item3
                                         }
                                 ).ToList();

                        successfulHRUsersProcessed.AddRange(processedUserSuccess);
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
                                             ID = personResults.Item1,
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
                log.Info("HR Records Processed: " + String.Format("{0:#,###0}", successfulHRUsersProcessed.Count));
                log.Info("HR Users Not Processed: " + String.Format("{0:#,###0}", unsuccessfulHRUsersProcessed.Count));
                log.Info("HR Processed Records: " + String.Format("{0:#,###0}", usersToProcess.Count));

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
            emailData.HRSuccessfulSummaryFilename = summaryFileGenerator.GenerateSummaryFile<ProcessedSummary, ProcessedSummaryMappng>(ConfigurationManager.AppSettings["SUCCESSSUMMARYFILENAME"].ToString(), usersProcessedSuccessSummary);
            emailData.HRErrorSummaryFilename = summaryFileGenerator.GenerateSummaryFile<ProcessedSummary, ProcessedSummaryMappng>(ConfigurationManager.AppSettings["ERRORSUMMARYFILENAME"].ToString(), usersProcessedErrorSummary);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="separationSuccessSummary"></param>
        /// <param name="separationErrorSummary"></param>
        private void GenerateSeparationSummaryFiles(List<SeperationSummary> separationSuccessSummary, List<SeperationSummary> separationErrorSummary)
        {
            emailData.SeparationSuccessfulSummaryFilename = summaryFileGenerator.GenerateSummaryFile<SeperationSummary, SeperationMapping>(ConfigurationManager.AppSettings["SEPARATIONSUMMARYFILENAME"].ToString(), separationSuccessSummary);
            emailData.SeparationErrorSummaryFilename = summaryFileGenerator.GenerateSummaryFile<SeperationSummary, SeperationMapping>(ConfigurationManager.AppSettings["SEPARATIONERRORSUMMARYFILENAME"].ToString(), separationErrorSummary);
        }

        internal void SendSummaryEMail()
        {
            EMail email = new EMail();
            StringBuilder emailAttachments = new StringBuilder();
            string subject = string.Empty;
            string body = string.Empty;            

            subject = ConfigurationManager.AppSettings["EMAILSUBJECT"].ToString() + " - " + DateTime.Now.ToString("MMMM dd, yyyy");

            body = GenerateEMailBody();

            //using (email)
            //{
            //    email.Send(ConfigurationManager.AppSettings["DEFAULTEMAIL"].ToString(), "", "", subject, "", body, "", "", true);
            //}
        }

        public string GenerateEMailBody()
        {
            StringBuilder errors = new StringBuilder();

            string template = File.ReadAllText(ConfigurationManager.AppSettings["SUMMARYTEMPLATE"]);

            //bool HRFileFound = false;
            //bool SEPFileFound = false;

            //HRFileFound = string.IsNullOrEmpty(emailData.HRFilename.ToString()) ? false : true;
            //SEPFileFound = string.IsNullOrEmpty(emailData.SEPFileName.ToString()) ? false : true;

            template = template.Replace("[FILENAMES]", emailData.HRFilename.ToString() + ", " + emailData.SEPFileName.ToString());

            template = template.Replace("[HRATTEMPTED]", emailData.HRAttempted.ToString());
            template = template.Replace("[HRSUCCEEDED]", emailData.HRSucceeded.ToString());
            template = template.Replace("[HRFAILED]", emailData.HRFailed.ToString());

            if (emailData.HRHasErrors)
            {
                errors.Clear();

                errors.Append("<b><font color='red'>Errors were found processing the HR file</font></b><br />");
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
            template = template.Replace("[SEPFAILED]", emailData.SEPFileName.ToString());

            if (emailData.SEPHasErrors)
            {
                errors.Clear();

                errors.Append("<b><font color='red'>Errors were found processing the separation file</font></b><br />");
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