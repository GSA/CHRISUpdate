using AutoMapper;
using CsvHelper;
using CsvHelper.Configuration;
using FluentValidation.Results;
using HRUpdate.Data;
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

        private readonly RetrieveData retrieve;

        private readonly SaveData save;

        private readonly EMailData emailData = new EMailData();

        private static Stopwatch timeForProcess = new Stopwatch();

        private enum Hrlinks { Separation = 1, Hrfile = 2 };

        //Constructor
        public ProcessData(IMapper dataMapper)
        {
            retrieve = new RetrieveData(dataMapper);
            save = new SaveData();
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

                HRSummary summary = new HRSummary();

                ValidateHR validate = new ValidateHR();

                Helpers helper = new Helpers();

                log.Info("Loading HR Links File");
                usersToProcess = GetFileData<Employee, EmployeeMapping>(HRFile);

                timeForProcess.Start();
                log.Info("Loading GCIMS Data");
                allGCIMSData = retrieve.AllGCIMSData();
                timeForProcess.Stop();

                log.Warn("It took " + timeForProcess.ElapsedMilliseconds + " to retrieve the data from the DB");

                Console.WriteLine("It took " + timeForProcess.ElapsedMilliseconds + " to retrieve the data from the DB");

                Tuple<int, string, string> updatedResults;

                //Start Processing the HR Data
                foreach (Employee employeeData in usersToProcess)
                {
                    log.Info("Processing HR User: " + employeeData.Person.EmployeeID);

                    //If there are critical errors write to the error summary and move to the next record
                    log.Info("Checking for Critical errors for user: " + employeeData.Person.EmployeeID);
                    if (CheckForErrors(validate, employeeData, summary.UnsuccessfulUsersProcessed))
                        continue;

                    CleanupHRData(employeeData);

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
                                summary.InactiveRecords.Add(new InactiveSummary
                                {
                                    GCIMSID = gcimsRecord.Person.GCIMSID,
                                    EmployeeID = employeeData.Person.EmployeeID,
                                    FirstName = employeeData.Person.FirstName,
                                    MiddleName = employeeData.Person.MiddleName,
                                    LastName = employeeData.Person.LastName,
                                    Suffix = employeeData.Person.Suffix,
                                    Status = employeeData.Person.Status
                                });

                                log.Warn("Inactive Record: " + employeeData.Person.EmployeeID);
                            }

                            log.Info("Updating Record: " + employeeData.Person.EmployeeID);

                            updatedResults = new Tuple<int, string, string>(-1, "Testing", "SQL Error (Testing)");

                            //updatedResults = save.UpdatePersonInformation(gcimsRecord.Person.GCIMSID, employeeData);

                            if (updatedResults.Item1 > 0)
                            {
                                summary.SuccessfulUsersProcessed.Add(new ProcessedSummary
                                {
                                    GCIMSID = updatedResults.Item1,
                                    EmployeeID = employeeData.Person.EmployeeID,
                                    FirstName = employeeData.Person.FirstName,
                                    MiddleName = employeeData.Person.MiddleName,
                                    LastName = employeeData.Person.LastName,
                                    Suffix = employeeData.Person.Suffix,
                                    Action = updatedResults.Item2
                                });

                                log.Info("Successfully Updated Record: " + employeeData.Person.EmployeeID);
                            }
                            else
                            {
                                summary.UnsuccessfulUsersProcessed.Add(new ProcessedSummary
                                {
                                    GCIMSID = gcimsRecord.Person.GCIMSID,
                                    EmployeeID = employeeData.Person.EmployeeID,
                                    FirstName = employeeData.Person.FirstName,
                                    MiddleName = employeeData.Person.MiddleName,
                                    LastName = employeeData.Person.LastName,
                                    Suffix = employeeData.Person.Suffix,
                                    Status = employeeData.Person.Status,
                                    Action = updatedResults.Item3
                                });

                                log.Error("Unable to update: " + employeeData.Person.EmployeeID);
                            }
                        }
                        else
                        {
                            log.Info("HR and GCIMS Data are the same: " + employeeData.Person.EmployeeID);

                            summary.SuccessfulUsersProcessed.Add(new ProcessedSummary
                            {
                                GCIMSID = gcimsRecord.Person.GCIMSID,
                                EmployeeID = employeeData.Person.EmployeeID,
                                FirstName = employeeData.Person.FirstName,
                                MiddleName = employeeData.Person.MiddleName,
                                LastName = employeeData.Person.LastName,
                                Suffix = employeeData.Person.Suffix,
                                Status = employeeData.Person.Status,
                                Action = "No update Required"
                            });
                        }
                    }
                    else
                    {
                        //Danger Will Robinson, Danger
                        summary.RecordNotFound.Add(new RecordNotFoundSummary
                        {
                            GCIMSID = -1,
                            EmployeeID = employeeData.Person.EmployeeID,
                            FirstName = employeeData.Person.FirstName,
                            MiddleName = employeeData.Person.MiddleName,
                            LastName = employeeData.Person.LastName,
                            Suffix = employeeData.Person.Suffix
                        });
                    }
                }

                emailData.HRFilename = Path.GetFileName(HRFile);
                emailData.HRAttempted = usersToProcess.Count;
                emailData.HRSocial = summary.SocialSecurityNumberChange.Count;
                emailData.HRSucceeded = summary.SuccessfulUsersProcessed.Count;
                emailData.HRInactive = summary.InactiveRecords.Count;
                emailData.HRRecordsNotFound = summary.RecordNotFound.Count;
                emailData.HRFailed = summary.UnsuccessfulUsersProcessed.Count;
                emailData.HRHasErrors = summary.UnsuccessfulUsersProcessed.Count > 0 ? true : false;

                //Add log entries
                log.Info("HR Records Updated: " + String.Format("{0:#,###0}", summary.SuccessfulUsersProcessed.Count));
                log.Info("HR Users Not Processed: " + String.Format("{0:#,###0}", summary.UnsuccessfulUsersProcessed.Count));
                log.Info("HR Total Records: " + String.Format("{0:#,###0}", usersToProcess.Count));

                GenerateUsersProccessedSummaryFiles(summary);
            }
            //Catch all errors
            catch (Exception ex)
            {
                log.Error("Process HR Users Error:" + ex.Message + " " + ex.InnerException + " " + ex.StackTrace);
            }
        }

        private bool CheckForErrors(ValidateHR validate, Employee employeeData, List<ProcessedSummary> unsuccessfulHRUsersProcessed)
        {
            ValidationResult criticalErrors;

            criticalErrors = validate.ValidateEmployeeCriticalInfo(employeeData);

            if (!criticalErrors.IsValid)
            {
                log.Warn("Errors found for user: " + employeeData.Person.EmployeeID + "(" + criticalErrors.Errors.Count() + ")");

                unsuccessfulHRUsersProcessed.Add(new ProcessedSummary
                {
                    GCIMSID = -1,
                    EmployeeID = employeeData.Person.EmployeeID,
                    FirstName = employeeData.Person.FirstName,
                    MiddleName = employeeData.Person.MiddleName,
                    LastName = employeeData.Person.LastName,
                    Suffix = employeeData.Person.Suffix,
                    Action = GetErrors(criticalErrors.Errors, Hrlinks.Hrfile).TrimEnd(',')
                });

                return true;
            }

            return false;
        }

        private void CleanupHRData(Employee employeeData)
        {
            employeeData.Phone.WorkFax.RemovePhoneFormatting();
            employeeData.Phone.WorkCell.RemovePhoneFormatting();
            employeeData.Phone.WorkPhone.RemovePhoneFormatting();
            employeeData.Phone.WorkTextTelephone.RemovePhoneFormatting();
            employeeData.Phone.HomeCell = employeeData.Phone.HomeCell.RemovePhoneFormatting();
            employeeData.Phone.HomePhone = employeeData.Phone.HomePhone.RemovePhoneFormatting();

            employeeData.Emergency.EmergencyContactHomePhone.RemovePhoneFormatting();
            employeeData.Emergency.EmergencyContactWorkPhone.RemovePhoneFormatting();
            employeeData.Emergency.EmergencyContactCellPhone.RemovePhoneFormatting();

            employeeData.Emergency.OutOfAreaContactHomePhone.RemovePhoneFormatting();
            employeeData.Emergency.OutOfAreaContactWorkPhone.RemovePhoneFormatting();
            employeeData.Emergency.OutOfAreaContactCellPhone.RemovePhoneFormatting();
        }

        private bool AreEqualGCIMSToHR(Employee GCIMSData, Employee HRData)
        {
            CompareLogic compareLogic = new CompareLogic();

            compareLogic.Config.TreatStringEmptyAndNullTheSame = true;

            compareLogic.Config.MembersToIgnore.Add("Person.GCIMSID");
            compareLogic.Config.MembersToIgnore.Add("Person.FirstName");
            compareLogic.Config.MembersToIgnore.Add("Person.MiddleName");
            compareLogic.Config.MembersToIgnore.Add("Person.LastName");
            compareLogic.Config.MembersToIgnore.Add("Person.Suffix");

            ComparisonResult result = compareLogic.Compare(GCIMSData, HRData);

            return result.AreEqual;
        }

        private Employee RecordFound(Employee employeeData, List<Employee> allGCIMSData)
        {
            var hrLinksMatch = allGCIMSData.Where(w => employeeData.Person.EmployeeID.Equals(string.IsNullOrEmpty(w.Person.EmployeeID) ? string.Empty : w.Person.EmployeeID)).ToList();

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
                log.Info("Trying to match record by Lastname, Birth Date and SSN: " + employeeData.Person.EmployeeID);

                var nameMatch = allGCIMSData.Where(w =>
                    employeeData.Person.LastName.ToLower().Trim().Equals(w.Person.LastName.ToLower().Trim()) &&
                    employeeData.Person.SocialSecurityNumber.Equals(w.Person.SocialSecurityNumber) &&
                    employeeData.Birth.DateOfBirth.Equals(w.Birth.DateOfBirth)).ToList();

                //var nameMatch = allGCIMSData.Where(c =>
                //    employeeData.Person.FirstName.ToLower().Trim().Equals(c.Person.FirstName.ToLower().Trim()) &&
                //    employeeData.Person.MiddleName.ToLower().Trim().Equals(string.IsNullOrEmpty(c.Person.MiddleName) ? string.Empty : c.Person.MiddleName.ToLower().Trim()) &&
                //    employeeData.Person.LastName.ToLower().Trim().Equals(c.Person.LastName.ToLower().Trim()) &&
                //    employeeData.Person.Suffix.ToLower().Trim().Equals(string.IsNullOrEmpty(c.Person.Suffix) ? string.Empty : c.Person.Suffix.ToLower().Trim()) &&
                //    employeeData.Person.SocialSecurityNumber.Equals(c.Person.SocialSecurityNumber) &&
                //    employeeData.Birth.DateOfBirth.Equals(c.Birth.DateOfBirth)).ToList();

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

                HRSeparationSummary summary = new HRSeparationSummary();

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

                            summary.SuccessfulUsersProcessed.Add(new SeparationSummary
                            {
                                GCIMSID = separationResults.Item1,
                                EmployeeID = separationData.EmployeeID,
                                SeparationCode = separationData.SeparationCode,
                                SeparationDate = separationData.SeparationDate,
                                Action = separationResults.Item3
                            });

                            log.Info("Successfully Separated Record: " + separationResults.Item1);
                        }
                        else
                        {
                            summary.UnsuccessfulUsersProcessed.Add(new SeparationSummary
                            {
                                GCIMSID = separationResults.Item1,
                                EmployeeID = separationData.EmployeeID,
                                SeparationCode = separationData.SeparationCode,
                                SeparationDate = separationData.SeparationDate,
                                Action = separationResults.Item3
                            });
                        }
                    }
                    else
                    {
                        summary.UnsuccessfulUsersProcessed.Add(new SeparationSummary
                        {
                            GCIMSID = -1,
                            EmployeeID = separationData.EmployeeID,
                            SeparationCode = separationData.SeparationCode,
                            SeparationDate = separationData.SeparationDate,
                            Action = GetErrors(errors.Errors, Hrlinks.Separation).TrimEnd(',')
                        });
                    }
                }

                emailData.SEPFileName = Path.GetFileName(SEPFile);
                emailData.SEPAttempted = separationUsersToProcess.Count;
                emailData.SEPSucceeded = summary.SuccessfulUsersProcessed.Count;
                emailData.SEPFailed = summary.UnsuccessfulUsersProcessed.Count;
                emailData.SEPHasErrors = summary.UnsuccessfulUsersProcessed.Count > 0 ? true : false;

                log.Info("Separation Records Processed: " + String.Format("{0:#,###0}", summary.SuccessfulUsersProcessed.Count));
                log.Info("Separation Users Not Processed: " + String.Format("{0:#,###0}", summary.UnsuccessfulUsersProcessed.Count));
                log.Info("Separation Total Records: " + String.Format("{0:#,###0}", separationUsersToProcess.Count));

                GenerateSeparationSummaryFiles(summary);
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
        private void GenerateUsersProccessedSummaryFiles(HRSummary summary)
        {
            if (summary.SuccessfulUsersProcessed.Count > 0)
            {
                emailData.HRSuccessfulFilename = summary.SummaryFileGenerator.GenerateSummaryFile<ProcessedSummary, ProcessedSummaryMapping>(ConfigurationManager.AppSettings["SUCCESSSUMMARYFILENAME"].ToString(), summary.SuccessfulUsersProcessed);
                log.Info("HR Success File: " + emailData.HRSuccessfulFilename);
            }

            if (summary.UnsuccessfulUsersProcessed.Count > 0)
            {
                emailData.HRUnsuccessfulFilename = summary.SummaryFileGenerator.GenerateSummaryFile<ProcessedSummary, ProcessedSummaryMapping>(ConfigurationManager.AppSettings["ERRORSUMMARYFILENAME"].ToString(), summary.UnsuccessfulUsersProcessed);
                log.Info("HR Error File: " + emailData.HRUnsuccessfulFilename);
            }

            if (summary.SocialSecurityNumberChange.Count > 0)
            {
                emailData.HRSocialSecurityNumberChangeFilename = summary.SummaryFileGenerator.GenerateSummaryFile<SocialSecurityNumberChangeSummary, SocialSecurityNumberChangeSummaryMapping>(ConfigurationManager.AppSettings["SOCIALSECURITYNUMBERCHANGESUMMARYFILENAME"].ToString(), summary.SocialSecurityNumberChange);
                log.Info("HR Social Security Number Change File: " + emailData.HRSocialSecurityNumberChangeFilename);
            }

            if (summary.InactiveRecords.Count > 0)
            {
                emailData.HRInactiveFilename = summary.SummaryFileGenerator.GenerateSummaryFile<InactiveSummary, InactiveSummaryMapping>(ConfigurationManager.AppSettings["INACTIVESUMMARYFILENAME"].ToString(), summary.InactiveRecords);
                log.Info("HR Inactive File: " + emailData.HRInactiveFilename);
            }

            if (summary.RecordNotFound.Count > 0)
            {
                emailData.HRRecordsNotFoundFileName = summary.SummaryFileGenerator.GenerateSummaryFile<RecordNotFoundSummary, RecordNotFoundSummaryMapping>(ConfigurationManager.AppSettings["RECORDNOTFOUNDSUMMARYFILENAME"].ToString(), summary.RecordNotFound);
                log.Info("HR Name Not Found File: " + emailData.HRInactiveFilename);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="separationSuccessSummary"></param>
        /// <param name="separationErrorSummary"></param>
        private void GenerateSeparationSummaryFiles(HRSeparationSummary summary)
        {
            if (summary.SuccessfulUsersProcessed.Count > 0)
            {
                emailData.SeparationSuccessfulFilename = summary.SummaryFileGenerator.GenerateSummaryFile<SeparationSummary, SeperationSummaryMapping>(ConfigurationManager.AppSettings["SEPARATIONSUMMARYFILENAME"].ToString(), summary.SuccessfulUsersProcessed);
                log.Info("Separation Success File: " + emailData.SeparationSuccessfulFilename);
            }

            if (summary.UnsuccessfulUsersProcessed.Count > 0)
            {
                emailData.SeparationErrorFilename = summary.SummaryFileGenerator.GenerateSummaryFile<SeparationSummary, SeperationSummaryMapping>(ConfigurationManager.AppSettings["SEPARATIONERRORSUMMARYFILENAME"].ToString(), summary.UnsuccessfulUsersProcessed);
                log.Info("Separation Error File: " + emailData.SeparationErrorFilename);
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
            template = template.Replace("[HRINACTIVE]", emailData.HRInactive.ToString());
            template = template.Replace("[HRFAILED]", emailData.HRFailed.ToString());

            if (emailData.HRHasErrors)
            {
                errors.Clear();

                errors.Append("<b><font color='red'>Errors were found while processing the HR file</font></b><br />");
                errors.Append("<br />Please see the attached file: <b><font color='red'>");
                errors.Append(emailData.HRUnsuccessfulFilename);
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
                errors.Append(emailData.SeparationErrorFilename);
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

            //HR Summary Files
            if (emailData.HRSuccessfulFilename != null)
                attachments.Append(AddAttachment(emailData.HRSuccessfulFilename));

            if (emailData.HRUnsuccessfulFilename != null)
                attachments.Append(AddAttachment(emailData.HRUnsuccessfulFilename));

            if (emailData.HRInactiveFilename != null)
                attachments.Append(AddAttachment(emailData.HRInactiveFilename));

            if (emailData.HRRecordsNotFoundFileName != null)
                attachments.Append(AddAttachment(emailData.HRRecordsNotFoundFileName));

            //Separation Summary Files
            if (emailData.SeparationSuccessfulFilename != null)
                attachments.Append(AddAttachment(emailData.SeparationSuccessfulFilename));

            if (emailData.SeparationErrorFilename != null)
                attachments.Append(AddAttachment(emailData.SeparationErrorFilename));

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