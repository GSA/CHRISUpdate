﻿using AutoMapper;
using CsvHelper;
using CsvHelper.Configuration;
using FluentValidation.Results;
using HRUpdate.Mapping;
using HRUpdate.Models;
using HRUpdate.Utilities;
using HRUpdate.Validation;
using KellermanSoftware.CompareNetObjects;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
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
        private readonly SaveData save;

        private readonly EMailData emailData = new EMailData();

        private static Stopwatch timeForProcess = new Stopwatch();

        //Constructor
        public ProcessData(IMapper saveMappper)
        {
            save = new SaveData(saveMappper);
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
                Employee gcimsRecord;

                List<Employee> usersToProcess;
                List<Employee> allGCIMSData;
                              
                List<ProcessedSummary> successfulHRUsersProcessed = new List<ProcessedSummary>();
                List<ProcessedSummary> unsuccessfulHRUsersProcessed = new List<ProcessedSummary>();
                List<SocialSecurityNumberChangeSummary> socialSecurityNumberChange = new List<SocialSecurityNumberChangeSummary>();
                List<RecordNotFoundSummary> recordsNotfound = new List<RecordNotFoundSummary>();
                List<InactiveSummary> inactive = new List<InactiveSummary>();

                ValidateHR validate = new ValidateHR();

                Helpers helper = new Helpers();

                log.Info("Loading HR Links File");
                usersToProcess = GetFileData<Employee, EmployeeMapping>(HRFile);

                timeForProcess.Start();
                log.Info("Loading GCIMS Data");
                allGCIMSData = save.LoadGCIMSData();
                timeForProcess.Stop();

                log.Warn("It took " + timeForProcess.ElapsedMilliseconds + " to retrieve the data from the DB");

                Console.WriteLine("It took " + timeForProcess.ElapsedMilliseconds + " to retrieve the data from the DB");

                Tuple<int, string, string> updatedResults;

                //Start Processing the HR Data
                foreach (Employee employeeData in usersToProcess)
                {
                    log.Info("Processing HR User: " + employeeData.Person.EmployeeID);

                    Console.WriteLine("Processing HR User: " + employeeData.Person.EmployeeID);

                    //If there are critical errors write to the error summary and move to the next record
                    log.Info("Checking for Critical errors for user: " + employeeData.Person.EmployeeID);
                    if (CheckForCriticalErrors(validate, employeeData, usersToProcess, unsuccessfulHRUsersProcessed))
                        continue;

                    //If there are non critical errors write to the error summary and continue with current record
                    log.Info("Checking for non critical errors for user: " + employeeData.Person.EmployeeID);
                    CheckForNonCriticalErrors(validate, employeeData, usersToProcess, unsuccessfulHRUsersProcessed);

                    //If record is found continue processing, otherwise record the issue
                    gcimsRecord = RecordFound(employeeData, allGCIMSData);

                    //If DB Record is not null them check if we need to update record
                    if (gcimsRecord != null)
                    {
                        log.Info("Comparing HR and GCIMS Data: " + employeeData.Person.EmployeeID);

                        if (!AreEqualGCIMSToHR(gcimsRecord, employeeData))
                        {
                            log.Info("Copying objects: " + employeeData.Person.EmployeeID);
                            helper.CopyValues<Employee>(employeeData, gcimsRecord);

                            log.Info("Checking if inactive record: " + employeeData.Person.EmployeeID);

                            if (employeeData.Person.Status == "Inactive")
                            {
                                inactive.Add(new InactiveSummary
                                {
                                    GCIMSID = employeeData.Person.GCIMSID,
                                    EmployeeID = employeeData.Person.EmployeeID,
                                    FirstName = employeeData.Person.FirstName,
                                    MiddleName = employeeData.Person.MiddleName,
                                    LastName = employeeData.Person.LastName
                                });
                            }

                            log.Info("Updating Record: " + employeeData.Person.EmployeeID);

                            updatedResults = new Tuple<int, string, string>(-1, "Testing", "SQL Error");
                            
                            //updatedResults = save.UpdatePersonInformation(employeeData.Person.GCIMSID, employeeData);

                            if (updatedResults.Item1 > 0)
                            {
                                successfulHRUsersProcessed.Add(new ProcessedSummary
                                {
                                    GCIMSID = employeeData.Person.GCIMSID,
                                    EmployeeID = employeeData.Person.EmployeeID,
                                    FirstName = employeeData.Person.FirstName,
                                    MiddleName = employeeData.Person.MiddleName,
                                    LastName = employeeData.Person.LastName,
                                    Action = updatedResults.Item2
                                });

                                log.Info("Successfully Updated Record: " + employeeData.Person.EmployeeID);
                            }
                            else
                            {
                                unsuccessfulHRUsersProcessed.Add(new ProcessedSummary
                                {
                                    GCIMSID = employeeData.Person.GCIMSID,
                                    EmployeeID = employeeData.Person.EmployeeID,
                                    FirstName = employeeData.Person.FirstName,
                                    MiddleName = employeeData.Person.MiddleName,
                                    LastName = employeeData.Person.LastName,
                                    Action = updatedResults.Item3
                                });

                                log.Error("Unable to update: " + employeeData.Person.EmployeeID);
                            }
                        }
                        else
                        {
                            log.Info("HR and GCIMS Data are the same: " + employeeData.Person.EmployeeID);

                            successfulHRUsersProcessed.Add(new ProcessedSummary
                            {
                                GCIMSID = employeeData.Person.GCIMSID,
                                EmployeeID = employeeData.Person.EmployeeID,
                                FirstName = employeeData.Person.FirstName,
                                MiddleName = employeeData.Person.MiddleName,
                                LastName = employeeData.Person.LastName,
                                Action = "No update Required"
                            });
                        }
                    }
                    else
                    {
                        //Danger Will Robinson, Danger
                        recordsNotfound.Add(new RecordNotFoundSummary
                        {
                            GCIMSID = -1,
                            EmployeeID = employeeData.Person.EmployeeID,
                            FirstName = employeeData.Person.FirstName,
                            MiddleName = employeeData.Person.MiddleName,
                            LastName = employeeData.Person.LastName,
                            Suffix = employeeData.Person.Suffix,
                            SSN = employeeData.Person.SocialSecurityNumber,
                            DOB = employeeData.Birth.DateOfBirth
                        });
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
                log.Info("HR Total Records: " + String.Format("{0:#,###0}", usersToProcess.Count));

                GenerateUsersProccessedSummaryFiles(successfulHRUsersProcessed, unsuccessfulHRUsersProcessed, socialSecurityNumberChange, inactive, recordsNotfound);
            }
            //Catch all errors
            catch (Exception ex)
            {
                log.Error("Process HR Users Error:" + ex.Message + " " + ex.InnerException + " " + ex.StackTrace);
            }
        }

        private bool CheckForCriticalErrors(ValidateHR validate, Employee employeeData, List<Employee> usersToProcess, List<ProcessedSummary> unsuccessfulHRUsersProcessed)
        {
            ValidationResult criticalErrors;

            criticalErrors = validate.ValidateEmployeeCriticalInfo(employeeData);

            if (!criticalErrors.IsValid)
            {
                log.Warn("Critical Errors found for user: " + employeeData.Person.EmployeeID + "(" + criticalErrors.Errors.Count() + ")");

                var proccessedUserIssue = usersToProcess
                        .Where(w => w.Person.EmployeeID == employeeData.Person.EmployeeID)
                        .Select
                             (
                                 s =>
                                     new ProcessedSummary
                                     {
                                         GCIMSID = -1,
                                         EmployeeID = s.Person.EmployeeID,
                                         FirstName = s.Person.FirstName,
                                         MiddleName = s.Person.MiddleName,
                                         LastName = s.Person.LastName,
                                         Action = "Critical - " + GetErrors(criticalErrors.Errors, Hrlinks.Hrfile).TrimEnd(',')
                                     }
                             ).ToList();

                unsuccessfulHRUsersProcessed.AddRange(proccessedUserIssue);

                return true;
            }

            return false;
        }

        private void CheckForNonCriticalErrors(ValidateHR validate, Employee employeeData, List<Employee> usersToProcess, List<ProcessedSummary> unsuccessfulHRUsersProcessed)
        {
            ValidationResult noncriticalErrors;

            noncriticalErrors = validate.validateEmployeeNonCriticalInfo(employeeData);

            if (!noncriticalErrors.IsValid)
            {
                log.Warn("Non critical errors found for user: " + employeeData.Person.EmployeeID + "(" + noncriticalErrors.Errors.Count() + ")");

                var proccessedUserIssue = usersToProcess
                   .Where(w => w.Person.EmployeeID == employeeData.Person.EmployeeID)
                   .Select
                        (
                            s =>
                                new ProcessedSummary
                                {
                                    GCIMSID = -1,
                                    EmployeeID = s.Person.EmployeeID,
                                    FirstName = s.Person.FirstName,
                                    MiddleName = s.Person.MiddleName,
                                    LastName = s.Person.LastName,
                                    Action = "Non-Critical - " + GetErrors(noncriticalErrors.Errors, Hrlinks.Hrfile).TrimEnd(',')
                                }
                        ).ToList();

                unsuccessfulHRUsersProcessed.AddRange(proccessedUserIssue);
            }
        }

        private bool AreEqualGCIMSToHR(Employee GCIMSData, Employee HRData)
        {
            CompareLogic compareLogic = new CompareLogic();
            compareLogic.Config.MembersToIgnore.Add("Person.SocialSecurityNumber");
            compareLogic.Config.MembersToIgnore.Add("Detail");
            compareLogic.Config.TreatStringEmptyAndNullTheSame = true;

            ComparisonResult result = compareLogic.Compare(GCIMSData, HRData);

            return result.AreEqual;
        }

        private Employee RecordFound(Employee employeeData, List<Employee> allGCIMSData)
        {
            var hrLinksMatch = allGCIMSData.Where(w => employeeData.Person.EmployeeID.Equals(string.IsNullOrEmpty(w.Person.EmployeeID) ? string.Empty : w.Person.EmployeeID)).ToList();

            //string.IsNullOrEmpty(employeeData.Person.FirstName) ? string.Empty : employeeData.Person.FirstName.ToLower().Trim()).Equals(string.IsNullOrEmpty(c.Person.FirstName) ? string.Empty : c.Person.FirstName.ToLower().Trim())

            if (hrLinksMatch.Count > 1)
            {
                log.Info("Multiple HR Links IDs Found: " + employeeData.Person.EmployeeID);

                return null;
            }
            else if (hrLinksMatch.Count == 1)
            {
                log.Info("Matching record found by emplID: " + employeeData.Person.EmployeeID);

                return hrLinksMatch.Single();
            }
            else if (hrLinksMatch.Count == 0)
            {
                log.Info("Trying to match record by FirstName, MiddleName, Lastname, Suffix, Birth Date and SSN: " + employeeData.Person.EmployeeID);

                var nameMatch = allGCIMSData.Where(c =>
                    employeeData.Person.FirstName.ToLower().Trim().Equals(c.Person.FirstName.ToLower().Trim()) &&
                    employeeData.Person.MiddleName.ToLower().Trim().Equals(string.IsNullOrEmpty(c.Person.MiddleName) ? string.Empty : c.Person.MiddleName.ToLower().Trim()) &&
                    employeeData.Person.LastName.ToLower().Trim().Equals(c.Person.LastName.ToLower().Trim()) &&
                    employeeData.Person.Suffix.ToLower().Trim().Equals(string.IsNullOrEmpty(c.Person.Suffix) ? string.Empty : c.Person.Suffix.ToLower().Trim()) &&
                    employeeData.Person.SocialSecurityNumber.Equals(c.Person.SocialSecurityNumber) &&
                    employeeData.Birth.DateOfBirth.Equals(c.Birth.DateOfBirth)).ToList();

                if (nameMatch.Count == 0 || nameMatch.Count > 1)
                {
                    log.Info("Match not found by name for user: " + employeeData.Person.EmployeeID);
                    return null;
                }
                else if (nameMatch.Count == 1)
                {
                    log.Info("Match found by name for user: " + employeeData.Person.EmployeeID);
                    return nameMatch.Single();
                }
            }

            return null;
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

                ValidateSeparation validate = new ValidateSeparation();
                ValidationResult errors;

                separationUsersToProcess = GetFileData<Separation, SeparationMapping>(SEPFile);

                Tuple<int, int, string, string> separationResults;

                foreach (Separation separationData in separationUsersToProcess)
                {
                    //Validate Record If Valid then process record
                    errors = validate.ValidateSeparationInformation(separationData);

                    if (errors.IsValid)
                    {
                        separationResults = save.SeparateUser(separationData);

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
                    else
                    {
                        var separationIssue = separationUsersToProcess
                                .Where(w => w.EmployeeID == separationData.EmployeeID)
                                .Select
                                    (
                                        s =>
                                            new SeperationSummary
                                            {
                                                GCIMSID = -1,
                                                EmployeeID = s.EmployeeID,
                                                SeparationCode = s.SeparationCode,
                                                Action = GetErrors(errors.Errors, Hrlinks.Separation).TrimEnd(',')
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
                log.Info("Separation Total Records: " + String.Format("{0:#,###0}", separationUsersToProcess.Count));

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
        private void GenerateUsersProccessedSummaryFiles(List<ProcessedSummary> usersProcessedSuccessSummary, List<ProcessedSummary> usersProcessedErrorSummary, List<SocialSecurityNumberChangeSummary> socialSecurityNumberChangeSummary, List<InactiveSummary> inactiveSummary, List<RecordNotFoundSummary> nameNotFoundSummary)
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

            if (socialSecurityNumberChangeSummary.Count > 0)
            {
                emailData.HRSocialSecurityNumberChangeSummaryFilename = summaryFileGenerator.GenerateSummaryFile<SocialSecurityNumberChangeSummary, SocialSecurityNumberChangeSummaryMapping>(ConfigurationManager.AppSettings["SOCIALSECURITYNUMBERCHANGESUMMARYFILENAME"].ToString(), socialSecurityNumberChangeSummary);
                log.Info("HR Social Security Number Change File: " + emailData.HRSocialSecurityNumberChangeSummaryFilename);
            }

            if (inactiveSummary.Count > 0)
            {
                emailData.HRErrorSummaryFilename = summaryFileGenerator.GenerateSummaryFile<InactiveSummary, InactiveSummaryMapping>(ConfigurationManager.AppSettings["INACTIVESUMMARYFILENAME"].ToString(), inactiveSummary);
                log.Info("HR Inactive File: " + emailData.HRInactiveSummaryFilename);
            }

            if (nameNotFoundSummary.Count > 0)
            {
                emailData.HRNameNotFoundFileName = summaryFileGenerator.GenerateSummaryFile<RecordNotFoundSummary, RecordNotFoundSummaryMapping>(ConfigurationManager.AppSettings["RECORDNOTFOUNDSUMMARYFILENAME"].ToString(), nameNotFoundSummary);
                log.Info("HR Name Not Found File: " + emailData.HRInactiveSummaryFilename);
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
                emailData.SeparationSuccessfulSummaryFilename = summaryFileGenerator.GenerateSummaryFile<SeperationSummary, SeperationSummaryMapping>(ConfigurationManager.AppSettings["SEPARATIONSUMMARYFILENAME"].ToString(), separationSuccessSummary);
                log.Info("Separation Success File: " + emailData.SeparationSuccessfulSummaryFilename);
            }

            if (separationErrorSummary.Count > 0)
            {
                emailData.SeparationErrorSummaryFilename = summaryFileGenerator.GenerateSummaryFile<SeperationSummary, SeperationSummaryMapping>(ConfigurationManager.AppSettings["SEPARATIONERRORSUMMARYFILENAME"].ToString(), separationErrorSummary);
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

            try
            {
                using (email)
                {
                    email.Send(ConfigurationManager.AppSettings["DEFAULTEMAIL"].ToString(),
                               ConfigurationManager.AppSettings["TO"].ToString(),
                               ConfigurationManager.AppSettings["CC"].ToString(),
                               ConfigurationManager.AppSettings["BCC"].ToString(),
                               subject, body, attahcments.TrimEnd(';'), ConfigurationManager.AppSettings["SMTPSERVER"].ToString(), true);
                }
            }
            catch (Exception ex)
            {
                log.Error("Error Sending HR Links Summary E-Mail: " + ex.Message + " - " + ex.InnerException);
            }
            finally
            {
                log.Info("HR Links Summary E-Mail Sent");
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

        private enum Hrlinks { Separation = 1, Hrfile = 2 };

        private string GetErrors(IList<ValidationFailure> failures, Hrlinks hr)
        {
            StringBuilder errors = new StringBuilder();

            foreach (var rule in failures)
            {
                errors.Append(rule.ErrorMessage.Remove(0, rule.ErrorMessage.IndexOf('.') + (int)hr));
                errors.Append(",");
            }

            return errors.ToString();
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