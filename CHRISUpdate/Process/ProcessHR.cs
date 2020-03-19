using AutoMapper;
using HRUpdate.Data;
using HRUpdate.Lookups;
using HRUpdate.Mapping;
using HRUpdate.Models;
using HRUpdate.Utilities;
using HRUpdate.Validation;
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
        private static log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static readonly string [] TerritoriesNotCountriesArray = { "rq", "gq", "vq", "aq" };

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
        /// 
        /// </summary>
        /// <param name="HRFile"></param>
        public void ProcessHRFile(string HRFile)
        {
            _log.Info("Processing HR Users");

            try
            {
                Employee gcimsRecord;
                var columnList = string.Empty;
                var summary = new HRSummary();
                var fileReader = new FileReader();
                var validate = new ValidateHR(lookups);
                var save = new SaveData();
                var em = new EmployeeMapping(lookups);
                List<string> badRecords;


                _log.Info("Loading HR Links File");
                
                
                var usersToProcess = fileReader.GetFileData<Employee, EmployeeMapping>(HRFile,out badRecords, em);
                Helpers.AddBadRecordsToSummary(badRecords, ref summary);

                _log.Info("Loading GCIMS Data");
                var allGCIMSData = retrieve.AllGCIMSData();

                ProcessResult updatedResults;

                //Start Processing the HR Data
                foreach (var employeeData in usersToProcess)
                {
                    _log.Info("Processing HR User: " + employeeData.Person.EmployeeID);

                    //Looking for matching record.
                    _log.Info("Looking for matching record: " + employeeData.Person.EmployeeID);
                    gcimsRecord = Helpers.RecordFound(employeeData, allGCIMSData, ref _log);

                    if ((gcimsRecord != null && (gcimsRecord.Person.EmployeeID != employeeData.Person.EmployeeID) && (!Convert.ToBoolean(ConfigurationManager.AppSettings["DEBUG"].ToString()))))
                    {
                        _log.Info("Adding HR Links ID to record: " + gcimsRecord.Person.GCIMSID);
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
                    _log.Info("Checking for Critical errors for user: " + employeeData.Person.EmployeeID);
                    if (Helpers.CheckForErrors(validate, employeeData, summary.UnsuccessfulUsersProcessed, ref _log))
                        continue;

                    Helpers.CleanupHrData(employeeData);

                    //If DB Record is not null them check if we need to update record
                    if (gcimsRecord != null)
                    {
                        //Hold the exclude list
                        var excludeList = new List<string>();
                        excludeList.AddRange(new[]{"InitialResult", "InitialResultDate", "FinalResult", "FinalResultDate"});

                        //Run personal phone number copy logic
                        var personalExcludeList = new[] {"HomePhone", "HomeCell", "WorkPhone", "WorkFax", "WorkCell", "WorkTextTelephone"};
                        excludeList.AddRange(personalExcludeList);
                        var eft = new ExcludedFieldTool("Phone");
                        eft.Create(
                            "Phone",
                            personalExcludeList,
                            employeeData
                        );
                        eft.Process(employeeData, gcimsRecord);

                        //Run emergency phone number copy logic
                        var emergencyExcludeList = new[] { "EmergencyContactName", "EmergencyContactHomePhone", "EmergencyContactWorkPhone", "EmergencyContactCellPhone", "OutOfAreaContactName", "OutOfAreaContactHomePhone", "OutOfAreaContactWorkPhone", "OutOfAreaContactCellPhone" };
                        excludeList.AddRange(emergencyExcludeList);
                        eft= new ExcludedFieldTool("Emergency");
                        eft.Create(
                            "Emergency",
                            emergencyExcludeList,
                            employeeData
                            );
                        eft.Process(employeeData, gcimsRecord);

                        //Run Home Addres copy logic
                        var homeAddressExclueList = new[] { "HomeAddress1", "HomeAddress2", "HomeAddress3",  "HomeCity", "HomeCountry", "HomeState", "HomeZipCode" };
                        excludeList.AddRange(homeAddressExclueList);
                        eft = new ExcludedFieldTool("HomeAddress");
                        eft.Create(
                            "HomeAddress",
                            homeAddressExclueList,
                            employeeData
                            );
                        eft.Process(employeeData, gcimsRecord);

                        Helpers.InvestigationCopy(employeeData, gcimsRecord);

                        _log.Info("Comparing HR and GCIMS Data: " + employeeData.Person.EmployeeID);

                        if (!Helpers.AreEqualGcimsToHr(gcimsRecord, employeeData, out columnList, ref _log))
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

                            _log.Info("Copying objects: " + employeeData.Person.EmployeeID);
                            Helpers.CopyValues(employeeData, gcimsRecord, excludeList.ToArray());
                            
                            _log.Info("Checking if inactive record: " + employeeData.Person.EmployeeID);

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

                                _log.Warn("Inactive Record: " + employeeData.Person.EmployeeID);
                            }

                            _log.Info("Updating Record: " + employeeData.Person.EmployeeID);

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
                                    Action = updatedResults.Action,
                                    UpdatedColumns = columnList
                                });

                                _log.Info("Successfully Updated Record: " + employeeData.Person.EmployeeID);
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

                                _log.Error("Unable to update: " + employeeData.Person.EmployeeID);
                            }
                        }
                        else
                        {
                            _log.Info("HR and GCIMS Data are the same: " + employeeData.Person.EmployeeID);

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
                _log.Info("HR Records Updated: " + $"{summary.SuccessfulUsersProcessed.Count:#,###0}");
                _log.Info("HR Users Not Processed: " + $"{summary.UnsuccessfulUsersProcessed.Count:#,###0}");
                _log.Info("HR Total Records: " + $"{usersToProcess.Count:#,###0}");

                summary.GenerateSummaryFiles(emailData);
            }
            //Catch all errors
            catch (Exception ex)
            {
                _log.Error("Process HR Users Error:" + ex.Message + " " + ex.InnerException + " " + ex.StackTrace);
            }
        }
    }
}