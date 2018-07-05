using AutoMapper;
using FluentValidation.Results;
using HRUpdate.Data;
using HRUpdate.Lookups;
using HRUpdate.Mapping;
using HRUpdate.Models;
using HRUpdate.Utilities;
using HRUpdate.Validation;
using KellermanSoftware.CompareNetObjects;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;

namespace HRUpdate.Process
{
    internal class ProcessHR
    {
        //Reference to logger
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static readonly string [] TerritoriesNotCountriesArray = new string[] { "rq", "gq", "vq", "aq" };

        private readonly RetrieveData retrieve;

        private readonly EMailData emailData;

        private enum Hrlinks { Separation = 1, Hrfile = 2 };

        readonly Lookup lookups;

        //Constructor
        public ProcessHR(IMapper dataMapper, ref EMailData emailData, Lookup lookups)
        {
            retrieve = new RetrieveData(dataMapper);

            this.lookups = lookups;

            this.emailData = emailData;
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
                FileReader fileReader = new FileReader();

                ValidateHR validate = new ValidateHR(lookups);

                Helpers helper = new Helpers();

                SaveData save = new SaveData();

                log.Info("Loading HR Links File");
                EmployeeMapping em = new EmployeeMapping(lookups);
                usersToProcess = fileReader.GetFileData<Employee, EmployeeMapping>(HRFile,em);

                log.Info("Loading GCIMS Data");
                allGCIMSData = retrieve.AllGCIMSData();

                ProcessResult updatedResults;

                //Start Processing the HR Data
                foreach (Employee employeeData in usersToProcess)
                {
                    log.Info("Processing HR User: " + employeeData.Person.EmployeeID);

                    //Looking for matching record.
                    log.Info("Looking for matching record: " + employeeData.Person.EmployeeID);
                    gcimsRecord = RecordFound(employeeData, allGCIMSData);

                    if ((gcimsRecord != null && (gcimsRecord.Person.EmployeeID != employeeData.Person.EmployeeID) && (!Convert.ToBoolean(ConfigurationManager.AppSettings["DEBUG"].ToString()))))
                    {
                        log.Info("Adding HR Links ID to record: " + gcimsRecord.Person.GCIMSID);
                        save.InsertEmployeeID(gcimsRecord.Person.GCIMSID, employeeData.Person.EmployeeID);
                    }

                    //If no record found write to the record not found summary file
                    if (gcimsRecord == null)
                    {
                        //Danger Will Robinson, Danger
                        summary.RecordsNotFound.Add(new RecordNotFoundSummary
                        {
                            GCIMSID = -1,
                            EmployeeID = employeeData.Person.EmployeeID,
                            FirstName = employeeData.Person.FirstName,
                            MiddleName = employeeData.Person.MiddleName,
                            LastName = employeeData.Person.LastName,
                            Suffix = employeeData.Person.Suffix
                        });
                    }

                    if (TerritoriesNotCountriesArray.Contains(employeeData.Birth.CountryOfBirth.ToLower()) && string.IsNullOrWhiteSpace(employeeData.Birth.StateOfBirth))
                    {
                        switch (employeeData.Birth.CountryOfBirth.ToLower())
                        {
                            case "rq": { employeeData.Birth.StateOfBirth = "PR"; }
                                break;
                            case "gq": { employeeData.Birth.StateOfBirth = "GU"; }
                                break;
                            case "vq": { employeeData.Birth.StateOfBirth = "VI"; }
                                break;
                            case "aq": { employeeData.Birth.StateOfBirth = "AS"; }
                                break;
                            default:   { employeeData.Birth.StateOfBirth = ""; }
                                break;
                        }
                        employeeData.Birth.CountryOfBirth = "US";
                    }
                    //If there are critical errors write to the error summary and move to the next record
                    log.Info("Checking for Critical errors for user: " + employeeData.Person.EmployeeID);
                    if (CheckForErrors(validate, employeeData, summary.UnsuccessfulUsersProcessed))
                        continue;

                    CleanupHRData(employeeData);

                    //If DB Record is not null them check if we need to update record
                    if (gcimsRecord != null)
                    {
                        InvestigationCopy(employeeData, gcimsRecord);

                        log.Info("Comparing HR and GCIMS Data: " + employeeData.Person.EmployeeID);

                        if (!AreEqualGCIMSToHR(gcimsRecord, employeeData))
                        {
                            //Checking if the SSN are different
                            if (employeeData.Person.SocialSecurityNumber != gcimsRecord.Person.SocialSecurityNumber)
                            {
                                summary.SocialSecurityNumberChanges.Add(new SocialSecurityNumberChangeSummary
                                {
                                    GCIMSID = gcimsRecord.Person.GCIMSID,
                                    EmployeeID = employeeData.Person.EmployeeID,
                                    FirstName = employeeData.Person.FirstName,
                                    MiddleName = employeeData.Person.MiddleName,
                                    LastName = employeeData.Person.LastName,
                                    Suffix = employeeData.Person.Suffix,
                                    Status = gcimsRecord.Person.Status
                                });
                            }

                            log.Info("Copying objects: " + employeeData.Person.EmployeeID);
                            helper.CopyValues<Employee>(employeeData, gcimsRecord, new string[] { "InitialResult", "InitialResultDate", "FinalResult", "FinalResultDate" });

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

                            if (Convert.ToBoolean(ConfigurationManager.AppSettings["DEBUG"].ToString()))
                            {
                                updatedResults = new ProcessResult
                                {
                                    Result = -1,
                                    Action = "Testing",
                                    Error = "SQL Error (Testing)"
                                };
                            }
                            else
                            {
                                updatedResults = save.UpdatePersonInformation(gcimsRecord.Person.GCIMSID, employeeData);
                            }

                            if (updatedResults.Result > 0)
                            {
                                summary.SuccessfulUsersProcessed.Add(new ProcessedSummary
                                {
                                    GCIMSID = gcimsRecord.Person.GCIMSID,
                                    EmployeeID = employeeData.Person.EmployeeID,
                                    FirstName = employeeData.Person.FirstName,
                                    MiddleName = employeeData.Person.MiddleName,
                                    LastName = employeeData.Person.LastName,
                                    Suffix = employeeData.Person.Suffix,
                                    Action = updatedResults.Action
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
                                    Action = updatedResults.Error
                                });

                                log.Error("Unable to update: " + employeeData.Person.EmployeeID);
                            }
                        }
                        else
                        {
                            log.Info("HR and GCIMS Data are the same: " + employeeData.Person.EmployeeID);

                            summary.IdenticalRecords.Add(new IdenticalRecordSummary
                            {
                                GCIMSID = gcimsRecord.Person.GCIMSID,
                                EmployeeID = employeeData.Person.EmployeeID,
                                FirstName = employeeData.Person.FirstName,
                                MiddleName = employeeData.Person.MiddleName,
                                LastName = employeeData.Person.LastName,
                                Suffix = employeeData.Person.Suffix,
                                Status = gcimsRecord.Person.Status
                            });
                        }
                    }
                }

                emailData.HRFilename = Path.GetFileName(HRFile);
                emailData.HRAttempted = usersToProcess.Count;
                emailData.HRIdentical = summary.IdenticalRecords.Count;
                emailData.HRSocial = summary.SocialSecurityNumberChanges.Count;
                emailData.HRSucceeded = summary.SuccessfulUsersProcessed.Count;
                emailData.HRInactive = summary.InactiveRecords.Count;
                emailData.HRRecordsNotFound = summary.RecordsNotFound.Count;
                emailData.HRFailed = summary.UnsuccessfulUsersProcessed.Count;
                emailData.HRHasErrors = summary.UnsuccessfulUsersProcessed.Count > 0;

                //Add log entries
                log.Info("HR Records Updated: " + String.Format("{0:#,###0}", summary.SuccessfulUsersProcessed.Count));
                log.Info("HR Users Not Processed: " + String.Format("{0:#,###0}", summary.UnsuccessfulUsersProcessed.Count));
                log.Info("HR Total Records: " + String.Format("{0:#,###0}", usersToProcess.Count));

                summary.GenerateSummaryFiles(emailData);
            }
            //Catch all errors
            catch (Exception ex)
            {
                log.Error("Process HR Users Error:" + ex.Message + " " + ex.InnerException + " " + ex.StackTrace);
            }
        }

        private void InvestigationCopy(Employee hr, Employee db)
        {
            bool? hrValue = hr.Investigation.InitialResult;
            DateTime? hrdate = hr.Investigation.InitialResultDate;
            bool? dbValue = db.Investigation.InitialResult;
            DateTime? dbdate = db.Investigation.InitialResultDate;

            if (hrValue == false || (hrdate == null || hrdate == DateTime.MinValue))
            {
                hr.Investigation.InitialResult = dbValue;
                hr.Investigation.InitialResultDate = dbdate;
            }

            hrValue = hr.Investigation.FinalResult;
            hrdate = hr.Investigation.FinalResultDate;
            dbValue = db.Investigation.FinalResult;
            dbdate = db.Investigation.FinalResultDate;

            if (hrValue == false || (hrdate == null || hrdate == DateTime.MinValue))
            {
                hr.Investigation.FinalResult = dbValue;
                hr.Investigation.FinalResultDate = dbdate;
            }
        }

        private bool CheckForErrors(ValidateHR validate, Employee employeeData, List<ProcessedSummary> unsuccessfulHRUsersProcessed)
        {
            ValidationResult criticalErrors;
            ValidationHelper validationHelper = new ValidationHelper();

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
                    Action = validationHelper.GetErrors(criticalErrors.Errors, ValidationHelper.Hrlinks.Hrfile).TrimEnd(',')
                });

                return true;
            }

            return false;
        }

        private void CleanupHRData(Employee employeeData)
        {
            Helpers helper = new Helpers();

            //Address clean up
            helper.cleanAddress(employeeData.Address);

            //Phone clean up
            employeeData.Phone.WorkFax = employeeData.Phone.WorkFax.RemovePhoneFormatting();
            employeeData.Phone.WorkCell = employeeData.Phone.WorkCell.RemovePhoneFormatting();
            employeeData.Phone.WorkPhone = employeeData.Phone.WorkPhone.RemovePhoneFormatting();
            employeeData.Phone.WorkTextTelephone = employeeData.Phone.WorkTextTelephone.RemovePhoneFormatting();
            employeeData.Phone.HomeCell = employeeData.Phone.HomeCell.RemovePhoneFormatting();
            employeeData.Phone.HomePhone = employeeData.Phone.HomePhone.RemovePhoneFormatting();

            //Emergency contact clean up
            employeeData.Emergency.EmergencyContactHomePhone = employeeData.Emergency.EmergencyContactHomePhone.RemovePhoneFormatting();
            employeeData.Emergency.EmergencyContactWorkPhone = employeeData.Emergency.EmergencyContactWorkPhone.RemovePhoneFormatting();
            employeeData.Emergency.EmergencyContactCellPhone = employeeData.Emergency.EmergencyContactCellPhone.RemovePhoneFormatting();
            employeeData.Emergency.OutOfAreaContactHomePhone = employeeData.Emergency.OutOfAreaContactHomePhone.RemovePhoneFormatting();
            employeeData.Emergency.OutOfAreaContactWorkPhone = employeeData.Emergency.OutOfAreaContactWorkPhone.RemovePhoneFormatting();
            employeeData.Emergency.OutOfAreaContactCellPhone = employeeData.Emergency.OutOfAreaContactCellPhone.RemovePhoneFormatting();
        }

        private bool AreEqualGCIMSToHR(Employee GCIMSData, Employee HRData)
        {
            CompareLogic compareLogic = new CompareLogic();

            compareLogic.Config.TreatStringEmptyAndNullTheSame = true;
            compareLogic.Config.CaseSensitive = false;
            compareLogic.Config.MaxDifferences = 100;
            compareLogic.Config.CustomComparers.Add(new EmployeeComparer(RootComparerFactory.GetRootComparer()));
            
            ComparisonResult result = compareLogic.Compare(GCIMSData, HRData);

            string[] diffs = result.Differences.Select(a => a.PropertyName).ToArray();
            string propertynamelist = string.Join(",", diffs);

            if (diffs != null && diffs.Length > 0)
            {
                log.Info(string.Format("Property differences include: {0}", propertynamelist));
            }

            return result.AreEqual;
        }

        private Employee RecordFound(Employee employeeData, List<Employee> allGCIMSData)
        {
            var hrLinksMatch = allGCIMSData.Where(w => employeeData.Person.EmployeeID == w.Person.EmployeeID).ToList();

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
    }
}