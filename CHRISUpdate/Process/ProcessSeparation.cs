using FluentValidation.Results;
using HRUpdate.Data;
using HRUpdate.Mapping;
using HRUpdate.Models;
using HRUpdate.Utilities;
using HRUpdate.Validation;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;

namespace HRUpdate.Process
{
    internal class ProcessSeparation
    {
        //Reference to logger
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly EMailData emailData;

        public ProcessSeparation(ref EMailData emailData)
        {
            this.emailData = emailData;
        }

        public void ProcessSeparationFile(string SEPFile)
        {
            log.Info("Processing Separation Users");
            
            try
            {
                SaveData save = new SaveData();

                List<Separation> separationUsersToProcess;

                FileReader fileReader = new FileReader();
                ValidationHelper validationHelper = new ValidationHelper();

                HRSeparationSummary summary = new HRSeparationSummary();

                ValidateSeparation validate = new ValidateSeparation();
                ValidationResult errors;

                SeparationResult separationResults;

                List<string> badRecords;

                separationUsersToProcess = fileReader.GetFileData<Separation, SeparationMapping>(SEPFile, out badRecords);

                foreach (Separation separationData in separationUsersToProcess)
                {
                    //Validate Record If Valid then process record
                    errors = validate.ValidateSeparationInformation(separationData);

                    if (errors.IsValid)
                    {
                        if (Convert.ToBoolean(ConfigurationManager.AppSettings["DEBUG"].ToString()))
                        {
                            separationResults = new SeparationResult
                            {
                                GCIMSID = -1,
                                Result = -1,
                                Action = "Testing",
                                SqlError = "SQL Error (Testing)"
                            };
                        }
                        else
                        {
                            separationResults = save.SeparateUser(separationData);
                        }

                        if (separationResults.GCIMSID > 0)
                        {
                            log.Info("Separating User: " + separationResults.GCIMSID);

                            summary.SuccessfulUsersProcessed.Add(new SeparationSummary
                            {
                                GCIMSID = separationResults.GCIMSID,
                                EmployeeID = separationData.EmployeeID,
                                FirstName = separationResults.FirstName,
                                MiddleName = separationResults.MiddleName,
                                LastName = separationResults.LastName,
                                Suffix = separationResults.Suffix,
                                SeparationCode = separationData.SeparationCode,
                                SeparationDate = separationData.SeparationDate,
                                Action = separationResults.Action
                            });

                            log.Info("Successfully Separated Record: " + separationResults.GCIMSID);
                        }
                        else
                        {
                            summary.UnsuccessfulUsersProcessed.Add(new SeparationSummary
                            {
                                GCIMSID = separationResults.GCIMSID,
                                EmployeeID = separationData.EmployeeID,
                                SeparationCode = separationData.SeparationCode,
                                SeparationDate = separationData.SeparationDate,
                                Action = separationResults.Action + " " + separationResults.SqlError
                            });

                            log.Info("Unsuccessfully Separated Record: " + separationData.EmployeeID);
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
                            Action = validationHelper.GetErrors(errors.Errors, ValidationHelper.Hrlinks.Separation).TrimEnd(',')
                        });

                        log.Info("Unsuccessfully Separated Record: " + separationData.EmployeeID);
                    }
                }

                emailData.SEPFileName = Path.GetFileName(SEPFile);
                emailData.SEPAttempted = separationUsersToProcess.Count;
                emailData.SEPSucceeded = summary.SuccessfulUsersProcessed.Count;
                emailData.SEPFailed = summary.UnsuccessfulUsersProcessed.Count;
                emailData.SEPHasErrors = summary.UnsuccessfulUsersProcessed.Count > 0;

                log.Info("Separation Records Processed: " + String.Format("{0:#,###0}", summary.SuccessfulUsersProcessed.Count));
                log.Info("Separation Users Not Processed: " + String.Format("{0:#,###0}", summary.UnsuccessfulUsersProcessed.Count));
                log.Info("Separation Total Records: " + String.Format("{0:#,###0}", separationUsersToProcess.Count));

                summary.GenerateSummaryFiles(emailData);
            }
            catch (Exception ex)
            {
                log.Error("Process Separation Users Error:" + ex.Message + " " + ex.InnerException);
            }
        }
    }
}